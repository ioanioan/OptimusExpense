using OptimusExpense.Data.Abstract;
using OptimusExpense.Infrastucture;
using OptimusExpense.Infrastucture.Utils;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class DocumentRepository:  EntityBaseRepository<OptimusExpense.Model.Models.Document>, IDocumentRepository
    {
        OptimusExpenseContext _context;
        IDocumentStateRepository _stateRepository;
        ISerialNumberRepository _serialNumberRepository;
        public DocumentRepository(OptimusExpenseContext c, IDocumentStateRepository stateRepository,
            ISerialNumberRepository serialNumberRepository) : base(c)
        {
            _context = c;
            _stateRepository = stateRepository;
            _serialNumberRepository = serialNumberRepository;
        }

        public override Document Save(Document entity)
        {
            int state = entity.StatusId;
            var userId = entity.CreatedByUserId;


            if (entity.StatusId == 0)
            {
                entity.StatusId = DictionaryDetailType.Generated.GetHashCode();            
            }
            if (entity.ServerDate == new DateTime())
            {
                entity.ServerDate = DateTime.Now;
            }
            if (entity.DocumentId > 0)
            {
               var exis= base.Get(entity.DocumentId);
                if (exis!=null)
                {
                    state = exis.StatusId;
                    entity.CreatedByUserId = exis.CreatedByUserId;
                }
                else
                {
                    state = 0;
                }
            }
            else
            {
                state = 0;
            }
            bool insG = false;
            if (entity.DocumentId == 0 && entity.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.Validated.GetHashCode())
            {
                insG = true;
            }
             base.Save(entity);
            if (insG)
            {
                _stateRepository.Save(new DocumentState
                {
                    DocumentId = entity.DocumentId,
                    TransitionMoment = DateTime.Now,
                    UserId = userId,
                    DocumentStateId = OptimusExpense.Infrastucture.DictionaryDetailType.Generated.GetHashCode()
                }); 
            }
            if (entity.StatusId != state)
            {
                _stateRepository.Save(new DocumentState
                {
                    DocumentId=entity.DocumentId,
                    TransitionMoment=DateTime.Now,
                    UserId= userId,
                    DocumentStateId=entity.StatusId
                });
            }
            return entity;
        }

       
    }
}
