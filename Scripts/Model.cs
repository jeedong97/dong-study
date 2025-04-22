using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status
{
    CardListOpen,
    CardListClose,
    PayMode,
    CouponListOpen,
    CardDetail,
    CardEditMode,
    KeyPad,
    SpareDetail,
    EditPage,
    MenuPage,
    MenuDepth2,
    PlusPage,
    Coibase,
    membershipDetail
}
public class Model : MonoBehaviour
{
    private static Model instance = null;
    public AnimationCurve Ease_InOut20_100;
    public AnimationCurve Ease_Out_100;
    public AnimationCurve Ease_In;
    public List<Sprite> ListAllSprite;
    public float DetailLimitPosition = 0;
    public float DetailLimitposition2 =0;

    void Awake()
    {
        // if (instance == null)
        // {
            instance = this;
        //     DontDestroyOnLoad(this.gameObject);
        // }
        // else
        // {
        //     Destroy(this.gameObject);
        // }
    }
    public static Model Inst
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
