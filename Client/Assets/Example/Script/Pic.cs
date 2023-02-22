using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.EventSystems;
public class Pic : UIBehaviour
{
    [Serializable]
    public class StatusData
    {
        public int status;
        public string statusDesc;
        public List<Graphic> graphics = new List<Graphic>();

        public StatusData(int _status, string _statusDesc)
        {
            status = _status;
            statusDesc = _statusDesc;
        }
    }
    public Camera camera1;
    public Camera camera2;
    public FixedSizeScrollGrid scroll;
    public GameObject can;
    private event System.Func<int> testF;
    protected override void Start()
    {
        // testF();
        // scroll.UpdateCount(10000, true, true);
        // scroll.onValueChanged.AddListener((position, normalize) =>
        // {
        //     Debug.Log(position + "   " + normalize);
        // });
        // if (can != null)
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        yield return new WaitForSeconds(2f);
        // GameObject go = Instantiate(can);
        // go.transform.parent = transform;
        scroll.UpdateCount(10000, true, true);
        yield return new WaitForSeconds(2f);
        scroll.ScrollTo(7, 0);
        // scroll.ScrollTo(100, 3);
    }

    protected override void OnRectTransformDimensionsChange()
    {

    }
}
