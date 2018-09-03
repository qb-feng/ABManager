using UnityEngine;
using System.Collections;
using Tangzx.ABSystem;
using HCFeng.ABManage.ABDownLoad;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    AssetBundleDownloadTaskModel downLoadModel;
    private RectTransform handle;
    Vector2 handleSize = Vector2.zero;
    private Vector2 parentSize = Vector2.zero;
    private Text handleMessage;
    // Use this for initialization
    void Start()
    {
        handle = transform.Find("Scrollbar/Sliding Area/Handle") as RectTransform;
        handleMessage = transform.Find("Scrollbar/Text").GetComponent<Text>();
        parentSize = (handle.parent.parent as RectTransform).sizeDelta;
        //开启自更新
        downLoadModel = AssetBundleDownloadManager.Instance.StarUpdateAssetBundle();
    }

    // Update is called once per frame
    void Update()
    {

        if (downLoadModel != null)
        {
            handleMessage.text = downLoadModel.taskMessage;

            if (downLoadModel.taskState != TaskState.None)
            {
                handleSize.x = parentSize.x * downLoadModel.Schedule;
                handle.sizeDelta = handleSize;
            }
            if (downLoadModel.taskState == TaskState.TaskOk)
            {
                DebugManager.LogWarning("ab 下载完成！卸载ab下载器！");
                AssetBundleDownloadManager.Instance.ExitUpdateAssetBundle();


                Invoke("LoadMainGame", 2f);
                downLoadModel = null;
            }
        }
    }


    private void LoadMainGame()
    {
        Destroy(handle.parent.parent.gameObject);
        var manager = gameObject.AddComponent<AssetBundleManager>();
        manager.Init(() =>
        {
            GameObject go = GameObject.Find("UIRoot");//a.Instantiate();
            AssetBundleManager.Instance.Load("Assets.Prefabs.UIHelloWorld.prefab", (b) =>
            {
                GameObject bo = Instantiate(b.mainObject) as GameObject;//a.Instantiate();
                bo.SetActive(false);
                RectTransform borect = bo.transform as RectTransform;
                borect.SetParent(go.transform);

                borect.localRotation = Quaternion.identity;
                borect.localScale = Vector3.one;

                bo.AddComponent<UIHelloWorld>();
                bo.SetActive(true);
            });

            AssetBundleManager.Instance.Load("Assets.Prefabs.Cube.prefab", (b) =>
            {
                GameObject bo = Instantiate(b.mainObject) as GameObject;//a.Instantiate();
                GameObject.FindObjectOfType<AAA>().len = 10;
            });




            //var bbb = Resources.Load<GameObject>("Cube");
            //GameObject qqq = Instantiate(bbb) as GameObject;//a.Instantiate();
        });



    }
}
