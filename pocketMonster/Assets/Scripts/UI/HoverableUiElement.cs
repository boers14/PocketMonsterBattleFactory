using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverableUiElement : MonoBehaviour
{
    public bool showUnder = false;

    [SerializeField]
    private Image bgForText = null;

    [SerializeField]
    private Text information = null;

    [SerializeField]
    private Sprite accuracySprite = null, physicalSprite = null, specialSprite = null, statusSprite = null;

    private List<Image> images = new List<Image>();
    private List<Text> texts = new List<Text>();

    private Sprite typeSprite = null;

    private bool informationIsCreated = false, setInformationWithImages = false;

    private string text = "", accuracyInfo = "", movePowerInfo = "";

    private float ySize = 6;

    private Color32 typeSpriteColor = Color.black;

    private PocketMonsterMoves.MoveSort moveSort = PocketMonsterMoves.MoveSort.Physical;

    public void PointerEnter()
    {
        if (!informationIsCreated)
        {
            informationIsCreated = true;
            GameObject canvas = GameObject.Find("Canvas");
            Image currentBg = Instantiate(bgForText);
            currentBg.transform.SetParent(canvas.transform);

            Vector2 size = Vector2.zero;
            size.x = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / ySize;
            currentBg.rectTransform.sizeDelta = size;

            Vector3 pos = Vector3.zero;
            pos.x = gameObject.GetComponent<RectTransform>().position.x;
            if (showUnder)
            {
                pos.y = gameObject.GetComponent<RectTransform>().position.y - gameObject.GetComponent<RectTransform>().sizeDelta.y / 2 - size.y / 2;
            } else
            {
                pos.y = gameObject.GetComponent<RectTransform>().position.y + gameObject.GetComponent<RectTransform>().sizeDelta.y / 2 + size.y / 2;
            }
            
            currentBg.rectTransform.position = pos;
            currentBg.color = Color.white;
            images.Add(currentBg);

            Text displayedInformation = Instantiate(information);
            displayedInformation.transform.SetParent(canvas.transform);
            displayedInformation.text = text;
            texts.Add(displayedInformation);

            if (!setInformationWithImages)
            {
                displayedInformation.rectTransform.sizeDelta = size;
                displayedInformation.rectTransform.position = pos;
            } else
            {
                Vector2 informationSize = Vector2.zero;
                informationSize.x = size.x * 0.67f;
                informationSize.y = size.y / 2;
                displayedInformation.rectTransform.sizeDelta = informationSize;
                Vector3 informationPos = Vector3.zero;
                informationPos.x = pos.x + size.x / 6;
                informationPos.y = pos.y - informationSize.y / 2;
                displayedInformation.rectTransform.position = informationPos;

                Text typeText = Instantiate(information);
                typeText.text = "Type: ";
                informationSize.x = size.x / 6;
                informationSize.y = size.y / 2;
                informationPos.y = pos.y + informationSize.y / 2;
                informationPos.x = pos.x - informationSize.x * 2 - informationSize.x / 2;
                CreateText(typeText, canvas, informationSize, informationPos);

                Vector2 imageSize = Vector2.zero;

                if (informationSize.x > informationSize.y)
                {
                    imageSize.x = informationSize.y;
                    imageSize.y = informationSize.y;
                } else
                {
                    imageSize.x = informationSize.x;
                    imageSize.y = informationSize.x;
                }

                Image typeImage = Instantiate(bgForText);
                typeImage.sprite = typeSprite;
                typeImage.color = typeSpriteColor;
                informationPos.x = pos.x - informationSize.x - informationSize.x / 2;
                CreateImage(typeImage, canvas, imageSize, informationPos);

                Image accuracyImage = Instantiate(bgForText);
                accuracyImage.sprite = accuracySprite;
                accuracyImage.color = Color.gray;
                informationPos.x = pos.x + informationSize.x + informationSize.x / 2;
                CreateImage(accuracyImage, canvas, imageSize, informationPos);

                Text accuracyText = Instantiate(information);
                accuracyText.text = ":" + accuracyInfo + "%";
                informationPos.x = pos.x + informationSize.x * 2 + informationSize.x / 2;
                CreateText(accuracyText, canvas, informationSize, informationPos);

                Text sortText = Instantiate(information);
                sortText.text = "Sort: ";
                informationPos.x = pos.x - informationSize.x * 2 - informationSize.x / 2;
                informationPos.y = pos.y - informationSize.y / 2;
                CreateText(sortText, canvas, informationSize, informationPos);

                Image sortImage = Instantiate(bgForText);
                Sprite sprite = physicalSprite;

                switch(moveSort)
                {
                    case PocketMonsterMoves.MoveSort.Physical:
                        sprite = physicalSprite;
                        break;
                    case PocketMonsterMoves.MoveSort.Special:
                        sprite = specialSprite;
                        break;
                    case PocketMonsterMoves.MoveSort.Status:
                        sprite = statusSprite;
                        break;
                }

                sortImage.color = Color.gray;
                sortImage.sprite = sprite;
                informationPos.x = pos.x - informationSize.x - informationSize.x / 2;
                CreateImage(sortImage, canvas, imageSize, informationPos);

                Text powerText = Instantiate(information);
                powerText.text = "Power: " + movePowerInfo;
                informationPos.x = pos.x;
                informationPos.y = pos.y + informationSize.y / 2;
                informationSize.x = size.x / 3;
                CreateText(powerText, canvas, informationSize, informationPos);

                for (int i = 0; i < texts.Count; i++)
                {
                    texts[i].alignment = TextAnchor.MiddleLeft;
                }
            }
        }
    }

    private void CreateText(Text text, GameObject canvas, Vector2 size, Vector3 pos)
    {
        text.transform.SetParent(canvas.transform);
        text.rectTransform.sizeDelta = size;
        text.rectTransform.position = pos;
        texts.Add(text);
    }


    private void CreateImage(Image image, GameObject canvas, Vector2 size, Vector3 pos)
    {
        image.transform.SetParent(canvas.transform);
        image.rectTransform.sizeDelta = size;
        image.rectTransform.position = pos;
        images.Add(image);
    }

    public void PointerExit()
    {
        informationIsCreated = false;

        for (int i = 0; i < images.Count; i++)
        {
            Destroy(images[i].gameObject);
        }
        images.Clear();

        for (int i = 0; i < texts.Count; i++)
        {
            Destroy(texts[i].gameObject);
        }
        texts.Clear();
    }

    public void SetSetInformationWithImages(bool setInformationWithImages)
    {
        this.setInformationWithImages = setInformationWithImages;
    }

    public void SetText(string text)
    {
        this.text = text;
    }

    public void SetTypeImageAndAccuracy(Sprite typeSprite, string accuracyInfo, Color32 typeSpriteColor, string movePowerInfo, 
        PocketMonsterMoves.MoveSort moveSort)
    {
        this.typeSprite = typeSprite;
        this.accuracyInfo = accuracyInfo;
        this.typeSpriteColor = typeSpriteColor;
        this.moveSort = moveSort;
        this.movePowerInfo = movePowerInfo;
    }

    public void SetYSize(float ySize)
    {
        this.ySize = ySize;
    }

    private void OnDestroy()
    {
        PointerExit();
    }

    private void OnDisable()
    {
        PointerExit();
    }
}
