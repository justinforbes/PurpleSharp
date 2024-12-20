﻿using PurpleSharp.Lib;
using System;
using System.Management;
using System.Threading;


namespace PurpleSharp.Simulations
{
    class Persistence
    {
        public static void CreateLocalAccountApi(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1136.001");
            logger.TimestampInfo("Using the Win32 API NetUserAdd function to execute the technique");
            try
            {
                PersistenceHelper.CreateUserApi(playbook_task.user, playbook_task.password, logger, playbook_task.cleanup);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void CreateLocalAccountCmd(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1136.001");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcessApi("", String.Format("net user {0} {1} /add", playbook_task.user, playbook_task.password), logger);
                Thread.Sleep(2000);
                if (playbook_task.cleanup)
                {
                    ExecutionHelper.StartProcessApi("", String.Format("net user {0} /delete", playbook_task.user), logger);
                }
                else
                {
                    logger.TimestampInfo(String.Format("The created local user {0} was not deleted as part of the simulation", playbook_task.user));
                }


                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void CreateScheduledTaskCmd(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1053.005");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcessApi("", String.Format(@"SCHTASKS /CREATE /SC DAILY /TN {0} /TR ""{1}"" /ST 13:00", playbook_task.taskName, playbook_task.taskPath), logger);
                if (playbook_task.cleanup)
                {
                    ExecutionHelper.StartProcessApi("", String.Format(@"SCHTASKS /DELETE /F /TN {0}", playbook_task.taskName), logger);
                    Thread.Sleep(3000);
                }
                else
                {
                    logger.TimestampInfo(@"The created Scheduled Task " + playbook_task.taskName + " was not deleted as part of the simulation");
                }
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }


        }
        public static void CreateRegistryRunKeyNET(string log, bool cleanup)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1547.001");
            logger.TimestampInfo("Using the Microsoft.Win32 .NET namespace to execute the technique");

            try
            {
                PersistenceHelper.RegistryRunKey(logger, cleanup);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
            
        }

        public static void CreateRegistryRunKeyCmd(string log, bool cleanup)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1547.001");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                string regKey = "BadApp";
                string binpath = @"C:\Windows\Temp\xyz12345.exe";

                ExecutionHelper.StartProcessApi("", String.Format(@"REG ADD HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run /V {0} /t REG_SZ /F /D {1}", regKey, binpath), logger);
                if (cleanup)
                {
                    Thread.Sleep(3000);
                    ExecutionHelper.StartProcessApi("", String.Format(@"REG DELETE HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run /V {0} /F", regKey), logger);
                }
                else
                {
                    logger.TimestampInfo(@"The created RegKey : HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\" + regKey + " was not deleted as part of the simulation");
                }
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }

            
        }

        public static void CreateWindowsServiceApi(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1543.003");
            logger.TimestampInfo("Using the Win32 API CreateService function to execute the technique");

            try
            {
                PersistenceHelper.CreateServiceApi(playbook_task, logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }

            
        }
        public static void CreateWindowsServiceCmd(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1543.003");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcessApi("", String.Format(@"sc create {0} binpath= {1} type= own start= auto", playbook_task.serviceName, playbook_task.servicePath), logger);
                Thread.Sleep(3000);
                if (playbook_task.cleanup) ExecutionHelper.StartProcessApi("", String.Format(@"sc delete {0}", playbook_task.serviceName), logger);
                else logger.TimestampInfo(String.Format("The created Service: {0} ImagePath: {1} was not deleted as part of the simulation", playbook_task.serviceName, playbook_task.servicePath));
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }  
        }

        public static void WMIEventSubscription(string log, bool cleanup)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1546.003");
            logger.TimestampInfo("Using the System.Management .NEt namespace to execute the technique");

            string wmiSubscription = "MaliciousWmiSubscription";
            //string vbscript64 = "<INSIDE base64 encoded VBS here>";
            //string vbscript = Encoding.UTF8.GetString(Convert.FromBase64String(vbscript64));
            try
            {
                ManagementObject EventFilter = null;
                ManagementObject EventConsumer = null;
                ManagementObject myBinder = null;

                ManagementScope scope = new ManagementScope(@"\\.\root\subscription");

                ManagementClass wmiEventFilter = new ManagementClass(scope, new
                ManagementPath("__EventFilter"), null);
                String strQuery = @"SELECT * FROM __InstanceCreationEvent WITHIN 5 " + "WHERE TargetInstance ISA \"Win32_Process\" " + "AND TargetInstance.Name = \"notepad.exe\"";

                WqlEventQuery myEventQuery = new WqlEventQuery(strQuery);
                EventFilter = wmiEventFilter.CreateInstance();
                EventFilter["Name"] = wmiSubscription;
                EventFilter["Query"] = myEventQuery.QueryString;
                EventFilter["QueryLanguage"] = myEventQuery.QueryLanguage;
                EventFilter["EventNameSpace"] = @"\root\cimv2";
                EventFilter.Put();
                logger.TimestampInfo(String.Format("EventFilter '{0}' created.", wmiSubscription));

                EventConsumer = new ManagementClass(scope, new ManagementPath("CommandLineEventConsumer"), null).CreateInstance();
                EventConsumer["Name"] = wmiSubscription;
                EventConsumer["CommandLineTemplate"] = "powershell.exe";
                EventConsumer.Put();
                logger.TimestampInfo(String.Format("CommandLineEventConnsumer '{0}' created.", wmiSubscription));

                /*
                EventConsumer = new ManagementClass(scope, new ManagementPath("ActiveScriptEventConsumer"), null).CreateInstance();
                EventConsumer["Name"] = "BadActiveScriptEventConsumer";
                EventConsumer["ScriptingEngine"] = "VBScript";
                EventConsumer["ScriptText"] = vbscript;
                EventConsumer.Put();
                */
                myBinder = new ManagementClass(scope, new ManagementPath("__FilterToConsumerBinding"), null).CreateInstance();
                myBinder["Filter"] = EventFilter.Path.RelativePath;
                myBinder["Consumer"] = EventConsumer.Path.RelativePath;
                myBinder.Put();

                logger.TimestampInfo("FilterToConsumerBinding created.");

                if (cleanup)
                {
                    Thread.Sleep(3 * 1000);
                    EventFilter.Delete();
                    EventConsumer.Delete();
                    myBinder.Delete();
                    logger.TimestampInfo("WMI Subscription Deleted");
                }
                else
                {
                    logger.TimestampInfo("The created WMI Subscription was not deleted as part of the simulation");
                }
                
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
                
            } 
        }
    }
}
