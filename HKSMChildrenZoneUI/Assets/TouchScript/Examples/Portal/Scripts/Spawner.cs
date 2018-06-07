/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Gestures;
using UnityEngine;

namespace TouchScript.Examples.Portal
{
    /// <exclude />
    public class Spawner : MonoBehaviour
    {

        public Transform Prefab;
        public Transform Position;

        private PressGesture pressGesture;

        private void OnEnable()
        {
            pressGesture = GetComponent<PressGesture>();
            pressGesture.Pressed += pressHandler;
        }

        private void OnDisable()
        {
            pressGesture.Pressed -= pressHandler;
        }

        private void pressHandler(object sender, EventArgs eventArgs)
        {
            var target = Instantiate(Prefab, Position.parent);
            target.position = Position.position;

            LayerManager.Instance.SetExclusive(target);
            pressGesture.Cancel(true, true);
            LayerManager.Instance.ClearExclusive();
        }
    }
}