using StructureMap;

namespace MethodFitness.Web.Config
{
    public class StructureMapBootstrapper : IBootstrapper
    {
        private static bool _hasStarted;

        public virtual void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry(new MFWebRegistry());
                                         });
            ObjectFactory.AssertConfigurationIsValid();
        }

        public static void Restart()
        {
            if (_hasStarted)
            {
                ObjectFactory.ResetDefaults();
            }
            else
            {
                Bootstrap();
                _hasStarted = true;
            }
        }

        public static void Bootstrap()
        {
            new StructureMapBootstrapper().BootstrapStructureMap();
        }
    }

    public class StructureMapBootstrapperTesting : IBootstrapper
    {
        private static bool _hasStarted;

        public virtual void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new MFTestRegistry());
            });
            ObjectFactory.AssertConfigurationIsValid();
        }


        public static void Restart()
        {
            if (_hasStarted)
            {
                ObjectFactory.ResetDefaults();
            }
            else
            {
                Bootstrap();
                _hasStarted = true;
            }
        }

        public static void Bootstrap()
        {
            new StructureMapBootstrapperTesting().BootstrapStructureMap();
        }
    }
}