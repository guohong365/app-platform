namespace Platform.Utils
{
    using System;
    using System.ServiceProcess;

    public sealed class ServiceUtility
    {
        public static ServiceController GetWindowsService(string serviceName)
        {
            return GetWindowsService(serviceName, true);
        }

        public static ServiceController GetWindowsService(string serviceName, bool ignoreCase)
        {
            foreach (ServiceController controller in ServiceController.GetServices())
            {
                if (string.Compare(controller.ServiceName, serviceName, ignoreCase) == 0)
                {
                    return controller;
                }
            }
            return null;
        }

        public static bool SetWindowsServiceStatus(ServiceController service, ServiceControllerStatus status)
        {
            return SetWindowsServiceStatus(service, status, 0);
        }

        public static bool SetWindowsServiceStatus(ServiceController service, ServiceControllerStatus status, int times)
        {
            if (service.Status == status)
            {
                return true;
            }
            if (times > 3)
            {
                return false;
            }
            ServiceControllerStatus status2 = status;
            if (status2 != ServiceControllerStatus.Stopped)
            {
                if (status2 != ServiceControllerStatus.Running)
                {
                    if (status2 == ServiceControllerStatus.Paused)
                    {
                        goto Label_011C;
                    }
                    goto Label_0187;
                }
                if (service.ServicesDependedOn != null)
                {
                    foreach (ServiceController controller in service.ServicesDependedOn)
                    {
                        if (!SetWindowsServiceStatus(controller, ServiceControllerStatus.Running))
                        {
                            return false;
                        }
                    }
                }
                try
                {
                    if (service.Status == ServiceControllerStatus.Paused)
                    {
                        service.Continue();
                    }
                    else
                    {
                        service.Start();
                    }
                }
                catch
                {
                    return false;
                }
                try
                {
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                    goto Label_0187;
                }
                catch
                {
                    goto Label_0187;
                }
            }
            if (service.DependentServices != null)
            {
                foreach (ServiceController controller2 in service.DependentServices)
                {
                    if (!SetWindowsServiceStatus(controller2, ServiceControllerStatus.Stopped))
                    {
                        return false;
                    }
                }
            }
            try
            {
                if (service.CanStop)
                {
                    service.Stop();
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            try
            {
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                goto Label_0187;
            }
            catch
            {
                goto Label_0187;
            }
        Label_011C:
            if (service.DependentServices != null)
            {
                foreach (ServiceController controller3 in service.DependentServices)
                {
                    if (!SetWindowsServiceStatus(controller3, ServiceControllerStatus.Stopped))
                    {
                        return false;
                    }
                }
            }
            try
            {
                if (service.CanPauseAndContinue)
                {
                    service.Pause();
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            try
            {
                service.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.FromSeconds(60));
            }
            catch
            {
            }
        Label_0187:
            if (service.Status != status)
            {
                times++;
                return SetWindowsServiceStatus(service, status, times);
            }
            return true;
        }
    }
}
