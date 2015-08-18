using System;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace CarsCatalog.Helpers
{
    public class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly IUnityContainer _container;

        public UnityControllerFactory(IUnityContainer container)
        {
            _container = container;
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)_container.Resolve(controllerType);
        }
    }
}