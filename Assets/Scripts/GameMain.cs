using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameMain : MonoBehaviour
{
    public Button[] buttonArray;              //持有所有Buton引用的数组
    private int firstId = -1, secondId = -1;  //用来保存翻开的两张牌的Id
    private Button firstClickBtn = null, secondClickBtn = null;//被点击的第一个第二个按钮
    private int cardFrontNum = 0;            //牌被翻开的数量(翻开两张时不能翻开第三张，直到检查出现结果)
    private int[] CardArray = { 1,2,3,4,5,6,7,8,9
                               ,1,2,3,4,5,6,7,8,9 }; //1~9代表9张牌的编号，Id=1,2,3...
    // Use this for initialization
    void Start()
    {
        //1.打乱数组的顺序
        //2.翻开所有牌
        InitCardArray();                      //将数组顺序打乱
        StartCoroutine(AllCardReverse());     //全部翻面，记忆
        
        
    }
    private IEnumerator AllCardReverse()
    {
        yield return new WaitForSeconds(0.2f);//准备时间
        for (int i = 0; i < buttonArray.Length; i++)
        {
            StartCoroutine(FrontToReverseAnima(buttonArray[i] , CardArray[i]));
        }
        yield return new WaitForSeconds(3.0f); //记忆并等待动画播放
        for (int i = 0; i < buttonArray.Length; i++)
        {
            buttonArray[i].GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites/0");
        }
        LoginButtonAddListener();
    }
    private void InitCardArray() //打乱数组 
    {

        //将数组元素顺序打乱
        for (int i = 0; i < CardArray.Length; i++)
        {
            int random = Random.Range(0, CardArray.Length);

            int temp = CardArray[i];
            CardArray[i] = CardArray[random];
            CardArray[random] = temp;
        }
    }          
    private void LoginButtonAddListener()  //注册按钮点击事件
    {
        /*****************巨坑*************/
        for (int i = 0; i < CardArray.Length; i++)
        {
            Button button = buttonArray[i];
            int temp = CardArray[i];
            button.onClick.AddListener(delegate () { AllButtonClick(button, temp); });
        }
        

    }
   
    private void AllButtonClick(Button btn , int resourcesId)
    {
        //按钮点击事件逻辑
        //1.显示翻转牌动画(DOtween)
        //2.动态地改变Sprites

        if (cardFrontNum < 2) //翻开的牌少于两张时，才可以翻牌
        {
            //播放正面到反面的动画
            StartCoroutine(FrontToReverseAnima(btn, resourcesId));
            if (cardFrontNum == 1)
            {

               
                secondId = resourcesId;
                secondClickBtn = btn;
                cardFrontNum++;
                btn.enabled = false;
                //print("two" + secondClickBtn.name);
                StartCoroutine(CheckCard());
            }
            if (cardFrontNum == 0)
            {
                
                firstId = resourcesId;
                firstClickBtn = btn;
                cardFrontNum++;
                btn.enabled = false;
                //print("one" + firstClickBtn.name);
            }
          
        }

        //print(btn.name + "Be Click");
    }
    private IEnumerator CheckCard()  //检查牌
    {
        yield return new WaitForSeconds(1.0f);
        if (cardFrontNum == 2)
        {
            if (firstId != secondId)
            {
                StartCoroutine(ReverseToFrontAnima(firstClickBtn, secondClickBtn));
            }
            if (firstId == secondId)
            {
                firstClickBtn.enabled = false;
                secondClickBtn.enabled = false;
                firstClickBtn.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites/" + firstId + "B");
                secondClickBtn.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites/" + secondId + "B");

                firstId = -1;
                secondId = -1;
                cardFrontNum = 0;
            }
            
        }
        else
        {
            Debug.LogError("未知错误" + "Line 202" + "cardFrontNum:" + cardFrontNum);
        }
    }  

    private IEnumerator FrontToReverseAnima(Button btn, int resourcesId) //正面翻转到反面的动画
    {
        btn.transform.DOLocalRotate(new Vector3(0, 85, 0), 0.5f);
        
        yield return new WaitForSeconds(0.5f);
        btn.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites/"+ resourcesId + "A");
        btn.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.Flash);
    }
    private IEnumerator ReverseToFrontAnima(Button btn_1 , Button btn_2) //反面翻转正面到的动画
    {
        btn_2.transform.DOLocalRotate(new Vector3(0, 85, 0), 0.5f);
        btn_1.transform.DOLocalRotate(new Vector3(0, 85, 0), 0.5f);
       

        yield return new WaitForSeconds(0.5f);

        btn_2.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites/0");
        btn_2.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.Flash);

        btn_1.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites/0");
        btn_1.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.Flash);

       
        firstId = -1;
        secondId = -1;
        cardFrontNum = 0;
        btn_1.enabled = true;
        btn_2.enabled = true;
    }
}
