using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwakeCam
{
    public class MouseEventManager : MonoBehaviour
    {
        public class MouseEvent
        {
            public enum Type { DEFAULT, OVER, DRAG, DOWN, UP, ENTER, EXIT }
            public Type type { get; }

            public Vector3 screenPosition { get; }
            public Vector3 screenDelta { get; }

            public Vector3 worldPosition { get; }
            public Vector3 worldDelta { get; }

            public MouseEvent(Type _type, Vector3 _screenPosition, Vector3 _screenDelta, Vector3 _worldPosition, Vector3 _worldDelta)
            {
                type = _type; screenPosition = _screenPosition; screenDelta = _screenDelta; worldPosition = _worldPosition; worldDelta = _worldDelta;
            }
        }

        public Vector3 pos = new Vector3(200, 200, 0);

        public Vector3 mousePosition;
        private Vector3 lastMouseScreenPosition;
        private Vector3 lastMouseWorldPosition;
        private GameObject lastHitObject;

        void Update()
        {
            if (AwakeCam.Manager.debugModeEnabled)
                return;

#if UNITY_EDITOR
            mousePosition = Input.mousePosition;
#else
        mousePosition = Display.RelativeMouseAt(Input.mousePosition);
#endif

            Ray ray = AwakeCam.Manager.camSets[(int)mousePosition.z].textureCamera.ScreenPointToRay(mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MouseEvent.Type mouseEventType = MouseEvent.Type.DEFAULT;

                if (lastHitObject != hit.collider.gameObject)
                {
                    hit.collider.SendMessage("OnMouseEnter", mousePosition, SendMessageOptions.DontRequireReceiver);
                    mouseEventType = MouseEvent.Type.ENTER;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.SendMessage("OnMouseDown", mousePosition, SendMessageOptions.DontRequireReceiver);
                    mouseEventType = MouseEvent.Type.DOWN;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    hit.collider.SendMessage("OnMouseUp", mousePosition, SendMessageOptions.DontRequireReceiver);
                    mouseEventType = MouseEvent.Type.UP;
                }
                else if (Input.GetMouseButton(0))
                {
                    hit.collider.SendMessage("OnMouseDrag", mousePosition, SendMessageOptions.DontRequireReceiver);
                    mouseEventType = MouseEvent.Type.DRAG;
                }
                else
                {
                    hit.collider.SendMessage("OnMouseOver", mousePosition, SendMessageOptions.DontRequireReceiver);
                    mouseEventType = MouseEvent.Type.OVER;
                }


                MouseEvent mouseEvent = new MouseEvent(
                    mouseEventType,
                    mousePosition,
                    mousePosition - lastMouseScreenPosition,
                    hit.point,
                    hit.point - lastMouseWorldPosition
                );

                hit.collider.SendMessage("OnMouseEvent", mouseEvent, SendMessageOptions.DontRequireReceiver);

                lastMouseWorldPosition = hit.point;

                if (lastHitObject != hit.collider.gameObject)
                    lastHitObject = hit.collider.gameObject;
            }
            else
            {
                if (lastHitObject != null)
                {
                    lastHitObject.SendMessage("OnMouseExit", mousePosition, SendMessageOptions.DontRequireReceiver);

                    lastHitObject.SendMessage("OnMouseEvent", new MouseEvent(
                        MouseEvent.Type.EXIT,
                        mousePosition,
                        mousePosition - lastMouseScreenPosition,
                        hit.point,
                        hit.point - lastMouseWorldPosition
                    ), SendMessageOptions.DontRequireReceiver);

                    lastHitObject = null;
                }
            }

            lastMouseScreenPosition = mousePosition;
        }

        private void OnGUI()
        {
            if (Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.D))
                GUILayout.Label(mousePosition.x + ", " + mousePosition.y + ", " + mousePosition.z);
        }
    }
}