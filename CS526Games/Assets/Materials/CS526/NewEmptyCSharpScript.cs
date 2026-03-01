using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.InputSystem; 

public class GameFlowController : MonoBehaviour
{
    [Header("1. 拖拽你的分组对象")]
    public GameObject boardGroup;   // 拖 ViewBoard
    public GameObject classGroup;   // 拖 ViewClassroom
    public GameObject reportGroup;  // 拖 ViewReport (最后显示的那个)

    [Header("2. 黑板内容设置")]
    public TMP_Text blackboardText; // 拖 ViewBoard 上的 Text (TMP)
    
    [TextArea]
    public string[] lectureSlides;  // PPT 内容 (Element 0 是第一句话)

    // 0 = 初始看黑板
    // 1 = 教室全景
    // 2 = 正式讲课
    // 3 = 讲完回教室
    // 4 = 显示报告 (Report)
    private int gameState = 0; 
    private int currentSlideIndex = 0;

    void Start()
    {
        // 初始化：进入状态 0
        gameState = 0; 
        currentSlideIndex = 0;
        UpdateView();
    }

    void Update()
    {
        // 检测空格键
        bool spacePressed = false;
        if (Input.GetKeyDown(KeyCode.Space)) spacePressed = true;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) spacePressed = true;

        if (spacePressed)
        {
            HandleInteraction();
        }
    }

    void HandleInteraction()
    {
        // --- 核心流程控制 ---

        if (gameState == 0) // 初始 -> 去教室
        {
            gameState = 1;
        }
        else if (gameState == 1) // 教室 -> 去黑板讲课
        {
            gameState = 2;
            currentSlideIndex = 0; // 从第 0 页开始
        }
        else if (gameState == 2) // 讲课中
        {
            currentSlideIndex++; // 翻页

            // 如果 PPT 讲完了
            if (currentSlideIndex >= lectureSlides.Length)
            {
                gameState = 3; // 去状态 3 (回教室)
            }
        }
        else if (gameState == 3) // 讲完了在教室 -> 去看报告
        {
            gameState = 4; // 去状态 4 (显示 Report)
        }
        else if (gameState == 4)
        {
            // 已经是最后一步了，再按空格可能没反应，或者重置游戏
            // Debug.Log("游戏结束");
        }

        UpdateView(); // 刷新画面
    }

    void UpdateView()
    {
        // --- 画面刷新逻辑 ---

        // State 0: 【初始黑板】
        if (gameState == 0) 
        {
            ShowOnly(boardGroup); 
            // 这里可以显示 "按空格开始..." 
        }
        // State 1: 【看学生】
        else if (gameState == 1) 
        {
            ShowOnly(classGroup);
        }
        // State 2: 【讲 PPT】
        else if (gameState == 2) 
        {
            ShowOnly(boardGroup);
            
            // 更新 PPT 文字
            if (blackboardText != null && lectureSlides.Length > 0)
            {
                if (currentSlideIndex < lectureSlides.Length)
                {
                    blackboardText.text = lectureSlides[currentSlideIndex];
                }
            }
        }
        // State 3: 【讲完回教室】
        else if (gameState == 3) 
        {
            ShowOnly(classGroup);
        }
        // State 4: 【最终报告】
        else if (gameState == 4) 
        {
            ShowOnly(reportGroup); // 只显示 Report，其他都关掉
        }
    }

    // --- 终极辅助工具：只显示一个，隐藏其他所有 ---
    void ShowOnly(GameObject targetToShow)
    {
        // 1. 先把所有人都关掉 (防止重叠)
        if (boardGroup != null) boardGroup.SetActive(false);
        if (classGroup != null) classGroup.SetActive(false);
        if (reportGroup != null) reportGroup.SetActive(false);

        // 2. 再把目标打开
        if (targetToShow != null)
        {
            targetToShow.SetActive(true);
        }
    }
}