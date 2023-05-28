using System;
using Zenject;


namespace CatSimulator.CameraService
{
    public class CameraStatesFactory : IFactory<Type, ICameraState>
    {
        private readonly DiContainer container;


        public CameraStatesFactory(DiContainer container)
        {
            this.container = container;
        }
        
        public ICameraState Create(Type type)
        {
            ICameraState cameraState = (ICameraState)container.Instantiate(type);
            
            /*container.Bind(type)
                .FromInstance(cameraState)
                .AsSingle();*/

            return cameraState;
        }
    }
}