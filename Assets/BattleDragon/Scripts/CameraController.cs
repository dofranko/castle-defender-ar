using UnityEngine;
using System.Collections;

namespace sword_world
{
    enum MouseButtonDown
    {
        MBD_LEFT = 0,
        MBD_RIGHT,
        MBD_MIDDLE,
    };

    public class CameraController : MonoBehaviour
    {
        public GameObject focused_obj = null;
        public float speed = 10.0f;

        Vector2 old_pos, mouse_pos;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta != 0.0f)
            {
                this.mouseWheelEvent(delta);
            }

            if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_LEFT) ||
                Input.GetMouseButtonDown((int)MouseButtonDown.MBD_MIDDLE) ||
                Input.GetMouseButtonDown((int)MouseButtonDown.MBD_RIGHT))
            {
                old_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            this.mouseDragEvent(Input.mousePosition);
        }

        //Dolly
        public void mouseWheelEvent(float delta)
        {
            Vector3 focusToPosition = this.transform.position - this.focused_obj.transform.position;

            Vector3 post = focusToPosition * (1.0f + delta);

            if (post.magnitude > 0.01)
                this.transform.position = this.focused_obj.transform.position + post;

            return;
        }

        void mouseDragEvent(Vector3 mousePos)
        {
            if (Input.GetMouseButton((int)MouseButtonDown.MBD_RIGHT))
            {
                mouse_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y); 

                float dx = mouse_pos.x - old_pos.x;
                dx *= speed;

                //下面开始旋转，仅在水平方向上进行旋转
                transform.RotateAround(focused_obj.transform.position, Vector3.up, dx * Time.deltaTime);
            }

            this.old_pos = mousePos;

            return;
        }
    }
}