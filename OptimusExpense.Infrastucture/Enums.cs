using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Infrastucture
{
    public enum DictionaryType
    {
        ActionType=-17,
        PartnerType = -7
    }

    public enum PartnerType
    {
        OwnCompany = -43,
        Bank = -24,
        Manufacturer = -23,
        Provider = -22,
        Client = -21
    }

    public enum DictionaryDetailType
    {
        Menu = -49,
        Action = -50,
        Canceled = -41,
        Finished = -40,
        Validated = -39,
        Generated = -38,
        ApproveSup= -58,
        ApproveCont=-60,
        CanceledSup=-57,
        CanceledCont=-59

    }


    public enum DocumentTypeEnum
    {
        ExpenseReport=1,
        ExpenseAdvance = 2
    }

    public enum PropertyType
    {
        DecontObsCont=-6,
        DecontObsSup=-5,
        AdvanceObsSup = -7,
        AdvanceObsCont = -8
    }
}
