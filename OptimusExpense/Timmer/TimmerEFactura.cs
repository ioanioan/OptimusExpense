using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OptimusExpense.Data;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using System;

namespace OptimusExpense.Timmer
{
    public class TimmerEFactura: ITimmerEFactura
    {
        IServiceScopeFactory _services;
        IEF_RaportareRepository _eF_RaportareRepository;
        public string _server, _port;
        ILogger<TimmerEFactura> _logger;
        System.Timers.Timer timerEFactura;
        ILogRepository  _logRepository;

        private String userId = "c6d34af8-ba34-4f16-a4b5-4cd0815e9130";//user efactura
        public TimmerEFactura(IEF_RaportareRepository eF_RaportareRepository, IConfiguration configuration, ILogger<TimmerEFactura> logger, ILogRepository logRepository, IServiceScopeFactory services)
        {
             this._services = services;
            _eF_RaportareRepository = eF_RaportareRepository;
            _server = "" +  configuration["EFacturaServer"];
            _port = "" +  configuration["EFacturaPort"];
            _logger = logger;

            timerEFactura = new System.Timers.Timer();
            timerEFactura.Interval = 1000 * 3600;
 
            timerEFactura.AutoReset = true;
            _logRepository = logRepository;
        }

        public void UplodNewFacturi(FilterInfo param)
        {
            param.UserId = userId;
            _eF_RaportareRepository.UplodNewFacturi(param);
        }

        private void SendEFacturi(FilterInfo param)
        {
            param.UserId=userId;
            _eF_RaportareRepository.SendEFacturi(_server,_port,param);
        }

        private void GetStariEFacturi(FilterInfo param)
        {
            param.UserId = userId;
            _eF_RaportareRepository.GetStariEFacturi(_server, _port, param);
        }

        private void timerActualizareEFactura_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
      

           

            try
            {
                SetContext();
                LogMessage("timerActualizareEFactura inceput, dezactivez metoda ..."  );

                timerEFactura.Stop();
                timerEFactura.Elapsed -= this.timerActualizareEFactura_Elapsed;


                try
                {
                    LogMessage("Adaugare EFactura");

                    var param = new FilterInfo();
                    param.Date = System.DateTime.Now.Date.AddDays(-2);
                    param.DateEnd = System.DateTime.Now.Date;

                   

                    try
                    {
                        UplodNewFacturi(param);
                        Log("Incarca facturi", "Automat");
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Adaugare EFactura UplodNewFacturi" + ex.Message + ": " + ex.StackTrace);
                    }

                    try
                    {
                        SendEFacturi(param);
                        Log("Trimite facturi", "Automat");
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Adaugare EFactura SendEFacturi" + ex.Message + ": " + ex.StackTrace);
                    }

                    try
                    {
                        GetStariEFacturi(param);
                        Log("Preluare stari facturi", "Automat");
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Adaugare EFactura GetStariEFacturi" + ex.Message + ": " + ex.StackTrace);
                    }



                    LogMessage("Adaugare EFactura succes");
                }
                catch (Exception ex)
                {
                    LogMessage("Eroare la Adaugare EFactura: " + ex.Message + ": " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                LogMessage("Eroare la Adaugare EFactura: " + ex.Message + ": " + ex.StackTrace);
            }
            finally
            {
                LogMessage("timerActualizareEFactura sfarsit, activez metoda ...");

                timerEFactura.Elapsed += new System.Timers.ElapsedEventHandler(this.timerActualizareEFactura_Elapsed);
                timerEFactura.Start();
            }
        }

        private void LogMessage(string message)
        {

            var action = "EFactura timmer";
            Log(action, message);
        }

        private void SetContext()
        {
            var scope = _services.CreateScope();
            var _context = scope.ServiceProvider.GetService<OptimusExpenseContext>();
            _logRepository.SetContext(_context);
            _eF_RaportareRepository.SetContext(_context);
        }
        private void Log (String action,string message)
        {
         

            _logger.LogError(action, message, "");
        
           _logRepository.Save(new Model.Models.Log { Action = action, Date = DateTime.Now, Value = message, UserId = userId });
        }

        public void Start()
        {
            LogMessage("Start");
            timerEFactura.Elapsed += new System.Timers.ElapsedEventHandler(this.timerActualizareEFactura_Elapsed);
            timerEFactura.Start();
        }



    }

    public interface ITimmerEFactura
    {

        void Start();
    }
}
