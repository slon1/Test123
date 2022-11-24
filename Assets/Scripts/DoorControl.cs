using UnityEngine;
namespace Tes123 {
	public interface IInteractable {
		public void Interact();
	}
	public class DoorControl : MonoBehaviour, IInteractable {
		private Animation anim;
		private bool flag = false;
		public void Interact() {
			if (!anim.isPlaying) {
				anim.Play("Door_" + ((flag = !flag) ? "Open" : "Close"));
			}
		}
		
		void Start() {

			anim = GetComponent<Animation>();
		}

		private void OnDestroy() {
			anim = null;
		}
	}
}