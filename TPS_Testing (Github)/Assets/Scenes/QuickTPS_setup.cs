using UnityEngine;

public class QuickTPS_setup : MonoBehaviour
{
    #region Setups (by AedanUndea)
    [SerializeField] bool fpView, tutorial = true, showRay = true;
    [Header("Camera Settings")]
    [SerializeField][Range(-2, 2)] float camX = 1.25f;
    [SerializeField][Range(0, 1)] float camY = 0;
    [SerializeField][Range(0.1f, 4)] float lookSpd = 1;
    [Range(0, 4)] float fpViewHgth = 1.5f;
    [Range(0.5f, 1)] float fpViewDis = 0.5f;
    [Range(0, 4)] float tpViewHgth = 2;
    [Range(0, 10)] float tpViewDis = 4;
    [Header("Player Settings")]
    [SerializeField][Range(0.1f, 4)] float moveSpd = 1;
    [SerializeField][Range(0.5f, 2)] float jumpHgth = 1;
    [SerializeField][Range(2, 4)] float turnSpd = 2;

    Transform ray, pivot, tcam;
    Rigidbody rb;
    MeshRenderer mr_beam;
    Vector3 targetPos;
    Vector2 lookStart, lookDir, lookRot, moveStart, moveDir, joyStkInput;
    float scrnW = Screen.width / 2, scrnH = Screen.height / 2, lookBrake = 1, moveBrake = 1, tpVhgthMin, tpVdisMin, tpCamXmin, lkRotX, camZoomSpd;
    bool loaded, editor, looking, moving, hovering, jumped, jystkUsed, mobileUsed;
    int tchLook = -1, tchMove = -1;

    void Start()
    {
        editor = Application.isEditor;

        if (!GetComponentInChildren<Renderer>())//if contains no renderer
        {
            MeshFilter tempM = GameObject.CreatePrimitive(PrimitiveType.Capsule).GetComponent<MeshFilter>(),
            newM = gameObject.AddComponent<MeshFilter>();
            newM.sharedMesh = tempM.sharedMesh;
            gameObject.AddComponent<MeshRenderer>();
            GetComponent<MeshRenderer>().material = tempM.GetComponent<MeshRenderer>().material;
            Destroy(tempM.gameObject);
        }

        if (!GetComponent<Collider>()) gameObject.AddComponent<CapsuleCollider>();

        if (!GetComponent<Rigidbody>()) rb = gameObject.AddComponent<Rigidbody>();
        else rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        transform.eulerAngles = Vector3.zero;
        if (rb.mass == 1) rb.mass = 60;

        ray = new GameObject().transform;
        SetChildOf(transform, Vector3.zero, new Vector3(0, -0.9f, 0), ray);
        ray.name = "ray";

        if (GetGroundDis(new Vector3(2, 10, 2), -Vector3.up, Mathf.Infinity) == 99)
        {
            Transform tGround = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            tGround.GetComponent<MeshRenderer>().material.color = new Color(0.3f, 0.4f, 0.2f);
            tGround.position = -Vector3.up * 2;
            tGround.localScale = new Vector3(50, 1, 50);
        }

        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(beam.GetComponent<Collider>());
        SetChildOf(ray, Vector3.zero, Vector3.forward, beam.transform);
        beam.transform.localScale = new Vector3(0.05f, 0.05f, 2);
        mr_beam = beam.GetComponent<MeshRenderer>();
        mr_beam.material.color = Color.red;
        mr_beam.material.SetColor("_EmissionColor", Color.red);
        mr_beam.material.SetFloat("_Glossiness", 0);
        mr_beam.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr_beam.receiveShadows = false;
        mr_beam.name = "beam";


        pivot = new GameObject().transform;
        pivot.eulerAngles = transform.eulerAngles;
        pivot.position = ray.position;
        pivot.name = "pivot";
        if (FindObjectOfType<Camera>()) tcam = FindObjectOfType<Camera>().transform;
        else tcam = new GameObject().AddComponent<Camera>().transform;
        tcam.GetComponent<Camera>().nearClipPlane = 0.01f;
        SetChildOf(pivot, Vector2.zero, new Vector3(camX, tpViewHgth, -tpViewDis), tcam);
        tcam.LookAt(pivot.forward * 500 + pivot.up * fpViewHgth * transform.localScale.y);
        tpVhgthMin = tpViewHgth - 0.5f; tpVdisMin = tpViewDis; tpCamXmin = camX - 0.75f;

        if (name == "GameObject") name = "player";

        loaded = true;
    }

    void OnGUI()
    {
        if (tutorial)
        {
            GUI.Button(new Rect(scrnW / 2 - 70, scrnH - 25, 140, 50), "Move with Rmouse");
            GUI.Button(new Rect(scrnW + scrnW / 2 - 70, scrnH -25, 140, 50), "Look with Lmouse");
            GUI.Button(new Rect(scrnW / 2 - 25, scrnH - 150, 50, 50), "W");
            GUI.Button(new Rect(scrnW / 2 - 25, scrnH - 100, 50, 50), "S");
            GUI.Button(new Rect(scrnW / 2 - 75, scrnH - 100, 50, 50), "A");
            GUI.Button(new Rect(scrnW / 2 + 25, scrnH - 100, 50, 50), "D");
            if (GUI.Button(new Rect(Screen.width - 130, 35, 120, 50), "Close Tutorial")) tutorial = false;
        }
    }
    #endregion (by AedanUndea)

    #region Mobile & PC Controls (by AedanUndea)
    void Update()
    {
        if (loaded)
        {
            mr_beam.enabled = showRay;
            pivot.position = ray.position + pivot.up * camY * transform.localScale.y;

            camZoomSpd = lookBrake * 6 * Time.deltaTime;
            if (!fpView)
            {
                targetPos = new Vector3(camX, tpViewHgth, -tpViewDis);
                tcam.LookAt(pivot.forward * 500 + pivot.up * fpViewHgth * transform.localScale.y);
            }
            else targetPos = new Vector3(0, fpViewHgth, fpViewDis);

            if (V3Dis(tcam.localPosition, targetPos) < 0.1f)
            {
                tcam.localPosition = targetPos;
                if (fpView) pivot.eulerAngles = ray.eulerAngles;
            }
            else
            {
                tcam.localPosition = Vector3.Lerp(tcam.localPosition, targetPos, camZoomSpd);
                if (fpView) pivot.eulerAngles = Vector3.Lerp(pivot.eulerAngles, ray.eulerAngles, camZoomSpd);
            }

            joyStkInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (joyStkInput != Vector2.zero) jystkUsed = true;
            else jystkUsed = false;
            if (Input.touchCount > 0)
            {
                mobileUsed = true;
                foreach (Touch t in Input.touches)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        if (t.position.x > scrnW)
                        {
                            tchLook = t.fingerId;
                            lookStart = t.position;
                            looking = true;
                        }
                        else
                        {
                            tchMove = t.fingerId;
                            moveStart = t.position;
                            moving = true;

                        }
                    }
                    else if (t.phase == TouchPhase.Moved)
                    {
                        if (t.fingerId == tchLook)
                        {
                            if (looking)
                            {
                                lookDir = t.position - lookStart;
                                lookStart = Vector2.Lerp(lookStart, t.position, lookBrake * 4 * Time.deltaTime);
                            }
                        }
                        else if(t.fingerId == tchMove)
                        {
                            if (!hovering && moving) moveDir = Vector2.ClampMagnitude(t.position - moveStart, scrnH) / scrnH;
                        }
                    }
                    else if(t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                    {
                        if(t.fingerId == tchLook)
                        {
                            tchLook = -1;
                            looking = false;
                        }
                        else if(t.fingerId == tchMove)
                        {
                            tchMove = -1;
                            moving = false;
                            moveDir = Vector2.zero;
                        }
                    }
                }
            }
            else
            {
                mobileUsed = false;

                if (Input.anyKey)
                {
                    if (Input.GetMouseButtonDown(1))//R clicked
                    {
                        lookStart = Input.mousePosition;
                        if (lookStart.x > scrnW) looking = true;
                    }
                    if (Input.GetMouseButtonDown(0) || jystkUsed)//L clicked
                    {
                        moveStart = Input.mousePosition;
                        if (moveStart.x < scrnW || jystkUsed) moving = true;

                    }

                    if (Input.GetMouseButton(1))//lookStart dragged
                    {
                        if (looking)
                        {
                            lookDir = (Vector2)Input.mousePosition - lookStart;
                            lookStart = Vector2.Lerp(lookStart, (Vector2)Input.mousePosition, lookBrake * 4 * Time.deltaTime);
                        }
                    }
                    if (Input.GetMouseButton(0) || jystkUsed)//moveStart dragged or joystick pressed
                    {
                        if (!hovering && moving)
                        {
                            if (!jystkUsed)
                            {
                                moveDir = Vector2.ClampMagnitude((Vector2)Input.mousePosition - moveStart, scrnH) / scrnH;
                            }
                            else
                                moveDir = joyStkInput;
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!hovering)
                        {
                            rb.AddForce(6 * jumpHgth * rb.mass * transform.up, ForceMode.Impulse);
                            hovering = true;
                            Invoke(nameof(Jumped), 1);
                        }
                    }
                }
            }

            if (!mobileUsed)
            {
                if (looking)
                {
                    if (Input.GetMouseButtonUp(1))//lookStart stopped
                        looking = false;
                }
                if (moving && !hovering)
                {
                    if (!Input.GetMouseButton(0) && !jystkUsed)//moveStart stopped or joystick released
                    {
                        moving = false;
                        moveDir = Vector2.zero;
                    }
                }
            }

            lookRot += lookSpd * Time.deltaTime * new Vector2(-lookDir.y, lookDir.x);
            lookRot.x = Mathf.Clamp(lookRot.x, -80, 80);
            lkRotX = 1 - Mathf.Abs(lookRot.x) / 80;
            if (!fpView)
            {
                pivot.eulerAngles = lookRot;
                tpViewHgth = 0.5f + tpVhgthMin * lkRotX;
                if (lookRot.x < 0)
                {
                    tpViewDis = tpVdisMin * lkRotX;
                    camX = 0.5f + tpCamXmin * lkRotX;
                }
                else
                {
                    tpViewDis = tpVdisMin;
                    camX = 0.5f + tpCamXmin;
                }
            }
            else
            {
                tcam.eulerAngles = new Vector2(lookRot.x, transform.eulerAngles.y);
                transform.eulerAngles = new Vector2(0, lookRot.y);
            }

            if (!looking) lookDir = Vector2.Lerp(lookDir, Vector2.zero, lookBrake * 4 * Time.deltaTime);
            if (!moving) moveDir = Vector2.Lerp(moveDir, Vector2.zero, moveBrake * 4 * Time.deltaTime);

            if (jumped)
            {
                if (GetGroundDis(ray.position, -transform.up, 10) < 0.15f)
                {
                    moveDir = Vector2.zero;
                    hovering = false;
                    jumped = false;
                }
            }

            Vector2 m = 4 * moveSpd * Time.deltaTime * moveDir;

            Vector3 forward = pivot.forward, sideway = pivot.right;
            forward.y = 0; sideway.y = 0;
            if (looking)
            {
                forward = transform.forward;
                sideway = transform.right;
            }
            else if (!fpView && m != Vector2.zero && moving)//align player's rotation to camera's rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, pivot.eulerAngles.y, 0), lookBrake * turnSpd * 4 * Time.deltaTime);

            transform.position += forward * m.y + sideway * m.x;
        }
    }
    #endregion //(by AedanUndea)

    #region Functions (by AedanUndea)
    float GetGroundDis(Vector3 from, Vector3 dir, float dis)
    {
        Vector3 rPos = from + -Vector3.up * 99;
        if (Physics.Raycast(from, dir, out RaycastHit hit, dis) && hit.collider)//detect a collider downwards with ray
        {
            rPos = hit.point;
            //print($"{hit.transform.name}({V3Dis(rPos, from)})");
        }

        return V3Dis(rPos, from);
    }
    float V3Dis(Vector3 v1, Vector3 v2)
    {
        return Mathf.Max(0, Vector3.Distance(v1, v2));
    }

    void Jumped()
    {
        jumped = true;
    }

    void SetChildOf(Transform parent, Vector3 rot, Vector3 pos, Transform child)
    {
        child.SetParent(parent);
        child.localEulerAngles = rot;
        child.localPosition = pos;
    }
    #endregion //(by AedanUndea)
}
