using UnityEngine;


namespace Arenar.Character
{
	public interface ICharacterCameraComponent : ICharacterComponent
	{
		void CameraRotation(Vector2 direction);
	}
}