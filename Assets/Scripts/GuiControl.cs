using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tes123 {
    public class GuiControl : MonoBehaviour {
        
        public Transform Root;        
        public void Save() {
            LoadSaveControl.Save(Root);
        }
        public void Load() {
            LoadSaveControl.Load(Root);
        }
        public void Clear() {
			for (int i = 0; i < Root.childCount; i++) {
                Destroy(Root.GetChild(i).gameObject);
			} 
        }
    }

}
