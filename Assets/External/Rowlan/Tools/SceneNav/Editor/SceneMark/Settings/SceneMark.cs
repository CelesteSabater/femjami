using Rowlan.SceneNav;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.SceneMark
{
    [Serializable]
    public class SceneMark
    {
        public Vector3 cameraPosition;
        public Quaternion cameraRotation;

        public Vector3 sceneViewPosition;
        public Quaternion sceneViewRotation;

        public GameObject target;

        [SerializeField]
        public Texture2D snapshot;

        private SceneMark()
        {
        }

        public SceneMark(ZoomEvent zoomEvent)
        {
            this.cameraPosition = zoomEvent.cameraPosition;
            this.cameraRotation = zoomEvent.cameraRotation;

            this.sceneViewPosition = zoomEvent.sceneViewPosition;
            this.sceneViewRotation = zoomEvent.sceneViewRotation;

            this.target = zoomEvent.target;
            this.snapshot = zoomEvent.snapshot;
        }

        public SceneMark Clone()
        {
            SceneMark clone = new SceneMark();

            clone.cameraPosition = cameraPosition;
            clone.cameraRotation = cameraRotation;

            clone.sceneViewPosition = sceneViewPosition;
            clone.sceneViewRotation = sceneViewRotation;

            clone.target = target;
            clone.snapshot = TextureUtils.CopyTexture2D(snapshot, true);

            return clone;

        }

    }
}