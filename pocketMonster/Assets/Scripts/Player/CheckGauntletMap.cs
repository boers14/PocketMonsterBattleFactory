using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckGauntletMap : MonoBehaviour
{
    private GameManager gameManager;

    private TerrainManager terrainManager;

    [SerializeField]
    private Image image = null, player = null;

    [SerializeField]
    private Text textObject = null;

    [SerializeField]
    private float sizeOfIsland = 6, widthOfConnection = 20, scrollFactor = 2;

    private List<Image> terrainPieces = new List<Image>();
    private List<Image> connections = new List<Image>();
    private List<Image> icons = new List<Image>();
    private Image bg = null, playerImage = null;

    private List<Text> information = new List<Text>();

    private Vector3 prevMousePos = Vector3.zero;

    private int startLenghtOfRun = 2;
    private float scrollPoints = 0;

    bool mapIsActive = false;

    private string pocketMonsterText = "Pick a pocketmonster to add to you're team, but if you already have four remove one first if you would " +
    "like to add one.";
    private string teamBuffText = "Pick a teambuff to add to you're team, but when you have three remove one first then add one.";

    [SerializeField]
    private Sprite pocketMonsterSprite = null, itemSprite = null, moveSprite = null, teamBuffSprite = null, finalBattleSprite = null;

    [SerializeField]
    private Color32 pocketMonsterColor = Color.yellow, itemColor = Color.blue, moveColor = Color.magenta, teamBuffColor = Color.white,
        finalBattleColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            CreateMap();
        } else if (Input.GetMouseButton(0))
        {
            MoveTheGauntletMap();
        } else if (Input.GetKeyUp(KeyCode.V))
        {
            ClearMap();
        }

        prevMousePos = Input.mousePosition;
    }

    private void MoveTheGauntletMap()
    {
        if (!mapIsActive)
        {
            return;
        }

        GameObject canvas = GameObject.Find("Canvas");
        Vector3 mousePos = Input.mousePosition;
        Vector3 differenceBetweenMousePosses = prevMousePos - mousePos;

        if (terrainPieces[0].GetComponent<RectTransform>().localPosition.y - differenceBetweenMousePosses.y >
            canvas.GetComponent<RectTransform>().sizeDelta.y / 4 || terrainPieces[terrainPieces.Count - 1].GetComponent<RectTransform>().localPosition.y - 
            differenceBetweenMousePosses.y < -canvas.GetComponent<RectTransform>().sizeDelta.y / 4)
        {
            return;
        }

        float movement = differenceBetweenMousePosses.y * scrollFactor;

        MoveAllMapPieces(movement);

        scrollPoints -= movement;
    }

    private void MoveAllMapPieces(float movement)
    {
        for (int i = 0; i < terrainPieces.Count; i++)
        {
            Vector3 newPos = terrainPieces[i].GetComponent<RectTransform>().localPosition;
            newPos.y -= movement;
            terrainPieces[i].GetComponent<RectTransform>().localPosition = newPos;
        }

        for (int i = 0; i < icons.Count; i++)
        {
            Vector3 newPos = icons[i].GetComponent<RectTransform>().localPosition;
            newPos.y -= movement;
            icons[i].GetComponent<RectTransform>().localPosition = newPos;
        }

        for (int i = 0; i < connections.Count; i++)
        {
            Vector3 newPos = connections[i].GetComponent<RectTransform>().localPosition;
            newPos.y -= movement;
            connections[i].GetComponent<RectTransform>().localPosition = newPos;
        }

        for (int i = 0; i < information.Count; i++)
        {
            Vector3 newPos = information[i].GetComponent<RectTransform>().localPosition;
            newPos.y -= movement;
            information[i].GetComponent<RectTransform>().localPosition = newPos;
            information[i].GetComponent<HoverableUiElement>().PointerExit();
        }
    }

    private void CreateMap()
    {
        if (mapIsActive)
        {
            return;
        }

        mapIsActive = true;
        GameObject canvas = GameObject.Find("Canvas");

        Image currentBg = Instantiate(image);
        currentBg.transform.SetParent(canvas.transform);
        currentBg.GetComponent<RectTransform>().localPosition = Vector3.zero;
        currentBg.rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        bg = currentBg;

        if (terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.StartOfMap || terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.SecondPocketMonster)
        {
            CreateStartOfMap(canvas);
        } else if (terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.FirstMoveSelection || terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.SecondMoveSelection ||
            terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.ThirdPocketMonster)
        {
            CreateSmallGauntlet(canvas);
        } else if (terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.LastBattle)
        {
            scrollPoints = 0;
            CreateEndGame(canvas);
        } else
        {
            CreateGauntlet(canvas);
        }

        for(int i = 0; i < terrainPieces.Count; i++)
        {
            terrainPieces[i].transform.SetAsLastSibling();
        }

        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].transform.SetAsLastSibling();
        }

        playerImage.transform.SetAsLastSibling();

        for (int i = 0; i < information.Count; i++)
        {
            information[i].transform.SetAsLastSibling();
        }

        MoveAllMapPieces(-scrollPoints);
    }

    private void CreateStartOfMap(GameObject canvas)
    {
        CreateTwoTerrainPieces(canvas);

        playerImage = Instantiate(player);
        playerImage.transform.SetParent(canvas.transform);
        Vector2 playerImageSize = Vector2.zero;
        playerImageSize.x = terrainPieces[0].GetComponent<RectTransform>().sizeDelta.y;
        playerImageSize.y = terrainPieces[0].GetComponent<RectTransform>().sizeDelta.y;
        playerImage.GetComponent<RectTransform>().sizeDelta = playerImageSize;
        playerImage.GetComponent<RectTransform>().localPosition = terrainPieces[0].GetComponent<RectTransform>().localPosition;
        connections.Add(playerImage);

        for (int i = 0; i < terrainPieces.Count; i++)
        {
            Image icon = Instantiate(image);
            CreateIcon(icon, canvas, i);
            icon.sprite = pocketMonsterSprite;
            icon.color = pocketMonsterColor;

            Text info = Instantiate(textObject);
            CreateInformation(info, canvas, i);
            info.text = "Pocketmonster";
            info.GetComponent<HoverableUiElement>().SetText(pocketMonsterText);
        }
    }

    private void CreateSmallGauntlet(GameObject canvas)
    {
        float addedY = canvas.GetComponent<RectTransform>().sizeDelta.y / (2 + terrainManager.currentLenght) + canvas.GetComponent<RectTransform>().sizeDelta.y / sizeOfIsland;
        float currentY = -canvas.GetComponent<RectTransform>().sizeDelta.y / 2 + ((canvas.GetComponent<RectTransform>().sizeDelta.y / sizeOfIsland) / 2);
        float leftX = -canvas.GetComponent<RectTransform>().sizeDelta.x / sizeOfIsland;
        float rightX = canvas.GetComponent<RectTransform>().sizeDelta.x / sizeOfIsland;
        int addedValue = 0;

        for (int i = 0; i < 6; i++)
        {
            Image terrainPiece = Instantiate(image);
            if (i == 0 || i == 5)
            {
                SetUIPosition(terrainPiece.gameObject, canvas, sizeOfIsland, sizeOfIsland, 0, currentY);
                currentY += addedY;
            }
            else
            {
                switch (addedValue)
                {
                    case 0:
                        SetUIPosition(terrainPiece.gameObject, canvas, sizeOfIsland, sizeOfIsland, leftX, currentY);
                        addedValue++;
                        break;
                    case 1:
                        SetUIPosition(terrainPiece.gameObject, canvas, sizeOfIsland, sizeOfIsland, rightX, currentY);
                        addedValue = 0;
                        currentY += addedY;
                        break;
                    default:
                        print("out of range");
                        break;
                }
            }
            terrainPiece.color = Color.green;
            terrainPieces.Add(terrainPiece);
        }

        playerImage = Instantiate(player);
        playerImage.transform.SetParent(canvas.transform);
        Vector2 playerImageSize = Vector2.zero;
        playerImageSize.x = terrainPieces[0].GetComponent<RectTransform>().sizeDelta.y;
        playerImageSize.y = terrainPieces[0].GetComponent<RectTransform>().sizeDelta.y;
        playerImage.GetComponent<RectTransform>().sizeDelta = playerImageSize;

        if (terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.FirstMoveSelection)
        {
            playerImage.GetComponent<RectTransform>().localPosition = terrainPieces[0].GetComponent<RectTransform>().localPosition;
        }
        else if (terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.SecondMoveSelection)
        {
            if (terrainManager.path == LandingTeleporter.SpawnPosition.Left)
            {
                playerImage.GetComponent<RectTransform>().localPosition = terrainPieces[1].GetComponent<RectTransform>().localPosition;
            }
            else if (terrainManager.path == LandingTeleporter.SpawnPosition.Right)
            {
                playerImage.GetComponent<RectTransform>().localPosition = terrainPieces[2].GetComponent<RectTransform>().localPosition;
            }
        }
        else if (terrainManager.whatToSpawn == TerrainManager.MapChunksToSpawn.ThirdPocketMonster)
        {
            if (terrainManager.path == LandingTeleporter.SpawnPosition.Left)
            {
                playerImage.GetComponent<RectTransform>().localPosition = terrainPieces[3].GetComponent<RectTransform>().localPosition;
            }
            else if (terrainManager.path == LandingTeleporter.SpawnPosition.Right)
            {
                playerImage.GetComponent<RectTransform>().localPosition = terrainPieces[4].GetComponent<RectTransform>().localPosition;
            }
        }
        connections.Add(playerImage);

        addedValue = 1;

        for (int i = 0; i < terrainPieces.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 1; j <= 2; j++)
                {
                    SetConnectionPiece(canvas, i, j, 0);
                }
            } else  if (i < terrainPieces.Count - 3)
            {
                switch (addedValue)
                {
                    case 0:
                        for (int j = 1; j <= 2; j++)
                        {
                            SetConnectionPiece(canvas, i, j, 0);
                        }
                        break;
                    case 1:
                        for (int j = 1; j <= 2; j++)
                        {
                            SetConnectionPiece(canvas, i, j, 1);
                        }
                        break;
                }
            }
            else if (i < terrainPieces.Count - 1)
            {
                switch (addedValue)
                {
                    case 0:

                        SetConnectionPiece(canvas, i, 0, 1);
                        break;
                    case 1:
                        SetConnectionPiece(canvas, i, 0, 2);
                        break;
                }
            }

            Image icon = Instantiate(image);
            CreateIcon(icon, canvas, i);

            Text info = Instantiate(textObject);
            CreateInformation(info, canvas, i);

            if (i == 0 || i == terrainPieces.Count - 1)
            {
                info.text = "Pocketmonster";
                icon.sprite = pocketMonsterSprite;
                icon.color = pocketMonsterColor;
                info.GetComponent<HoverableUiElement>().SetText(pocketMonsterText);
            }
            else
            {
                int currentInLineOfGuantlets = 0;
                int extraMovePickupsDoneInGauntlet = 0;

                for (int j = 2; j < i; j += 2)
                {
                    currentInLineOfGuantlets++;
                }

                for (int j = 0; j < currentInLineOfGuantlets; j++)
                {
                    extraMovePickupsDoneInGauntlet += 2;
                }

                int index = extraMovePickupsDoneInGauntlet + (1 - addedValue);
                info.text = gameManager.allMovesHandedOut[index].moveName;
                icon.sprite = moveSprite;
                icon.color = moveColor;
                SetTextDescription(info, gameManager.allMovesHandedOut[index]);
                info.GetComponent<HoverableUiElement>().SetText(gameManager.allMovesHandedOut[index].moveDescription);

                if (addedValue == 0)
                {
                    addedValue = 2;
                }

                addedValue--;
            }
        }
    }

    private void CreateEndGame(GameObject canvas)
    {
        CreateTwoTerrainPieces(canvas);

        playerImage = Instantiate(player);
        playerImage.transform.SetParent(canvas.transform);
        int neededIndex = 0;

        if (gameManager.lastBattle)
        {
            neededIndex = 1;
        }

        Vector2 playerImageSize = Vector2.zero;
        playerImageSize.x = terrainPieces[neededIndex].GetComponent<RectTransform>().sizeDelta.y;
        playerImageSize.y = terrainPieces[neededIndex].GetComponent<RectTransform>().sizeDelta.y;
        playerImage.GetComponent<RectTransform>().sizeDelta = playerImageSize;
        playerImage.rectTransform.localPosition = terrainPieces[neededIndex].rectTransform.localPosition;
        connections.Add(playerImage);

        for (int i = 0; i < terrainPieces.Count; i++)
        {
            Image icon = Instantiate(image);
            CreateIcon(icon, canvas, i);

            Text info = Instantiate(textObject);
            CreateInformation(info, canvas, i);
            if (i == 0)
            {
                if (gameManager.nextBigPickUp == GameManager.BigPickups.PocketMonster)
                {
                    info.text = "Teambuff";
                    icon.sprite = teamBuffSprite;
                    icon.color = teamBuffColor;
                    info.GetComponent<HoverableUiElement>().SetText(teamBuffText);
                } else
                {
                    info.text = "Pocketmonster";
                    icon.sprite = pocketMonsterSprite;
                    icon.color = pocketMonsterColor;
                    info.GetComponent<HoverableUiElement>().SetText(pocketMonsterText);
                }
            } else
            {
                info.text = "Last battle";
                icon.sprite = finalBattleSprite;
                icon.color = finalBattleColor;
                info.GetComponent<HoverableUiElement>().SetText("Last battle before victory");
            }
        }
    }

    private void CreateGauntlet(GameObject canvas)
    {
        int amountOfIslands = 2 + (terrainManager.currentLenght * 3);
        float addedY = canvas.GetComponent<RectTransform>().sizeDelta.y / (2 + terrainManager.currentLenght) + canvas.GetComponent<RectTransform>().sizeDelta.y / sizeOfIsland;
        float currentY = -canvas.GetComponent<RectTransform>().sizeDelta.y / 2 + ((canvas.GetComponent<RectTransform>().sizeDelta.y / sizeOfIsland) / 2);
        float leftX = -canvas.GetComponent<RectTransform>().sizeDelta.x / sizeOfIsland;
        float rightX= canvas.GetComponent<RectTransform>().sizeDelta.x / sizeOfIsland;
        int addedValue = 0;

        for (int i = 0; i < amountOfIslands; i++)
        {
            Image terrainPiece = Instantiate(image);
            if (i == 0 || i == amountOfIslands - 1)
            {
                SetUIPosition(terrainPiece.gameObject, canvas, sizeOfIsland, sizeOfIsland, 0, currentY);
                currentY += addedY;
            }
            else
            {
                switch (addedValue)
                {
                    case 0:
                        SetUIPosition(terrainPiece.gameObject, canvas, sizeOfIsland, sizeOfIsland, leftX, currentY);
                        addedValue++;
                        break;
                    case 1:
                        SetUIPosition(terrainPiece.gameObject, canvas, sizeOfIsland, sizeOfIsland, 0, currentY);
                        addedValue++;
                        break;
                    case 2:
                        SetUIPosition(terrainPiece.gameObject, canvas, sizeOfIsland, sizeOfIsland, rightX, currentY);
                        addedValue = 0;
                        currentY += addedY;
                        break;
                    default:
                        print("out of range");
                        break;
                }
            }
            terrainPiece.color = Color.green;
            terrainPieces.Add(terrainPiece);
        }

        playerImage = Instantiate(player);
        playerImage.transform.SetParent(canvas.transform);
        Vector2 playerImageSize = Vector2.zero;
        playerImageSize.x = terrainPieces[0].GetComponent<RectTransform>().sizeDelta.y;
        playerImageSize.y = terrainPieces[0].GetComponent<RectTransform>().sizeDelta.y;
        playerImage.GetComponent<RectTransform>().sizeDelta = playerImageSize;

        if (terrainManager.currentPlaceInGuantlet == 0)
        {
            playerImage.rectTransform.localPosition = terrainPieces[0].rectTransform.localPosition;
        } else
        {
            int neededIndex = 0;
            if (terrainManager.path == LandingTeleporter.SpawnPosition.Left)
            {
                neededIndex = terrainManager.currentPlaceInGuantlet * 3 - 2;
            }
            else if (terrainManager.path == LandingTeleporter.SpawnPosition.Right)
            {
                neededIndex = terrainManager.currentPlaceInGuantlet * 3;
            }
            else if (terrainManager.path == LandingTeleporter.SpawnPosition.Middle)
            {
                neededIndex = terrainManager.currentPlaceInGuantlet * 3 - 1;
            }
            playerImage.rectTransform.localPosition = terrainPieces[neededIndex].rectTransform.localPosition;
        }
        connections.Add(playerImage);

        int startingPoint = terrainManager.currentLenght - startLenghtOfRun;
        int doneGauntlets = 0;
        int amountOfItemsDone = -1;
        int amountOfMovesDone = 3;
        addedValue = 2;

        for (int i = 0; i < startingPoint; i++)
        {
            doneGauntlets += 2 + i;
        }

        for (int i = 0; i < doneGauntlets; i++)
        {
            if (gameManager.pickups[i] == GameManager.PickupsInGauntlet.Item)
            {
                amountOfItemsDone += 3;
            }
            else
            {
                amountOfMovesDone += 3;
            }
        }

        for (int i = 0; i < terrainPieces.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 1; j <= 3; j++)
                {
                    SetConnectionPiece(canvas, i, j, 0);
                }
            }
            else if (i < terrainPieces.Count - 4)
            {
                switch (addedValue)
                {
                    case 0:
                        for (int j = 1; j <= 2; j++)
                        {
                            SetConnectionPiece(canvas, i, j, 1);
                        }
                        break;
                    case 1:
                        for (int j = 1; j <= 3; j++)
                        {
                            SetConnectionPiece(canvas, i, j, 1);
                        }
                        break;
                    case 2:
                        for (int j = 1; j <= 2; j++)
                        {
                            SetConnectionPiece(canvas, i, j, 2);
                        }
                        break;
                    default:
                        print("out of range");
                        break;
                }
            }
            else if (i < terrainPieces.Count - 1)
            {
                switch (addedValue)
                {
                    case 0:
                        SetConnectionPiece(canvas, i, 1, 0);
                        break;
                    case 1:
                        SetConnectionPiece(canvas, i, 2, 0);
                        break;
                    case 2:
                        SetConnectionPiece(canvas, i, 3, 0);
                        break;
                    default:
                        break;
                }
            }

            Image icon = Instantiate(image);
            CreateIcon(icon, canvas, i);

            Text info = Instantiate(textObject);
            CreateInformation(info, canvas, i);

            if (i == 0 || i == terrainPieces.Count - 1)
            {
                if (i == 0 && startingPoint == 0)
                {
                    info.text = "Pocketmonster";
                    icon.sprite = pocketMonsterSprite;
                    icon.color = pocketMonsterColor;
                    info.GetComponent<HoverableUiElement>().SetText(pocketMonsterText);
                } else if (i == 0)
                {
                    if (gameManager.nextBigPickUp == GameManager.BigPickups.PocketMonster)
                    {
                        info.text = "Teambuff";
                        icon.sprite = teamBuffSprite;
                        icon.color = teamBuffColor;
                        info.GetComponent<HoverableUiElement>().SetText(teamBuffText);
                    } else
                    {
                        info.text = "Pocketmonster";
                        icon.sprite = pocketMonsterSprite;
                        icon.color = pocketMonsterColor;
                        info.GetComponent<HoverableUiElement>().SetText(pocketMonsterText);
                    }
                } else if (i == terrainPieces.Count - 1)
                {
                    if (gameManager.nextBigPickUp == GameManager.BigPickups.PocketMonster)
                    {
                        info.text = "Pocketmonster";
                        icon.sprite = pocketMonsterSprite;
                        icon.color = pocketMonsterColor;
                        info.GetComponent<HoverableUiElement>().SetText(pocketMonsterText);
                    }
                    else
                    {
                        info.text = "Teambuff";
                        icon.sprite = teamBuffSprite;
                        icon.color = teamBuffColor;
                        info.GetComponent<HoverableUiElement>().SetText(teamBuffText);
                    }
                }
            } else
            {
                int currentInLineOfGuantlets = 0;
                int extraItemPickUpsDoneInGauntlet = 0;
                int extraMovePickupsDoneInGauntlet = 0;

                for (int j = 3; j < i; j += 3)
                {
                    currentInLineOfGuantlets++;
                }

                for (int j = doneGauntlets; j < doneGauntlets + currentInLineOfGuantlets; j++)
                {
                    if (gameManager.pickups[j] == GameManager.PickupsInGauntlet.Item)
                    {
                        extraItemPickUpsDoneInGauntlet += 3;
                    }
                    else if (gameManager.pickups[j] == GameManager.PickupsInGauntlet.Move)
                    {
                        extraMovePickupsDoneInGauntlet += 3;
                    }
                }

                if (gameManager.pickups[doneGauntlets + currentInLineOfGuantlets] == GameManager.PickupsInGauntlet.Item)
                {
                    int index = amountOfItemsDone + extraItemPickUpsDoneInGauntlet + (3 - addedValue);
                    info.text = gameManager.itemsToHandOut[index].name;
                    icon.sprite = itemSprite;
                    icon.color = itemColor;
                    info.GetComponent<HoverableUiElement>().SetText(gameManager.itemsToHandOut[index].itemDescription);
                }
                else
                {
                    int index = amountOfMovesDone + extraMovePickupsDoneInGauntlet + (3 - addedValue);
                    info.text = gameManager.allMovesHandedOut[index].moveName;
                    icon.sprite = moveSprite;
                    icon.color = moveColor;
                    SetTextDescription(info, gameManager.allMovesHandedOut[index]);
                    info.GetComponent<HoverableUiElement>().SetText(gameManager.allMovesHandedOut[index].moveDescription);
                }

                if (addedValue == 0)
                {
                    addedValue = 3;
                }

                addedValue--;
            }
        }
    }

    private void CreateInformation(Text info, GameObject canvas, int i)
    {
        info.transform.SetParent(canvas.transform);
        Vector2 textSize = Vector2.zero;
        textSize.x = terrainPieces[i].GetComponent<RectTransform>().sizeDelta.x;
        textSize.y = terrainPieces[i].GetComponent<RectTransform>().sizeDelta.y / 2;
        info.GetComponent<RectTransform>().sizeDelta = textSize;
        Vector3 textPos = Vector3.zero;
        textPos.x = terrainPieces[i].GetComponent<RectTransform>().localPosition.x;
        textPos.y = terrainPieces[i].GetComponent<RectTransform>().localPosition.y + textSize.y / 2;
        info.GetComponent<RectTransform>().localPosition = textPos;
        information.Add(info);
    }

    private void CreateIcon(Image icon, GameObject canvas, int i)
    {
        icon.transform.SetParent(canvas.transform);
        Vector2 iconSize = Vector2.zero;
        iconSize.x = terrainPieces[i].GetComponent<RectTransform>().sizeDelta.y / 1.4f;
        iconSize.y = terrainPieces[i].GetComponent<RectTransform>().sizeDelta.y / 1.4f;
        icon.GetComponent<RectTransform>().sizeDelta = iconSize;
        Vector3 iconPos = Vector3.zero;
        iconPos.x = terrainPieces[i].GetComponent<RectTransform>().localPosition.x;
        iconPos.y = terrainPieces[i].GetComponent<RectTransform>().localPosition.y - iconSize.y / 4f;
        icon.GetComponent<RectTransform>().localPosition = iconPos;
        icons.Add(icon);
    }

    private void SetConnectionPiece(GameObject canvas, int i, int j, int addedValue)
    {
        Image connectionPiece = Instantiate(image);
        connectionPiece.transform.SetParent(canvas.transform);
        Vector3 pos = Vector3.zero;
        Vector2 size = Vector2.zero;

        float addedY = terrainPieces[i + (j + addedValue)].rectTransform.localPosition.y -
            terrainPieces[i].rectTransform.localPosition.y;
        float addedX = terrainPieces[i + (j + addedValue)].rectTransform.localPosition.x -
            terrainPieces[i].rectTransform.localPosition.x;

        pos.y = terrainPieces[i].rectTransform.localPosition.y + addedY / 2;
        pos.x = terrainPieces[i].rectTransform.localPosition.x + addedX / 2;

        size.y = Mathf.Sqrt(Mathf.Pow(addedY, 2) + Mathf.Pow(addedX, 2));
        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / widthOfConnection;

        connectionPiece.rectTransform.localPosition = pos;
        connectionPiece.rectTransform.sizeDelta = size;

        Vector3 lookAt = new Vector3(terrainPieces[i + (j + addedValue)].rectTransform.localPosition.x,
           terrainPieces[i + (j + addedValue)].rectTransform.localPosition.y, 0);
        float angleRad = Mathf.Atan2(lookAt.y - connectionPiece.rectTransform.localPosition.y, lookAt.x -
            connectionPiece.rectTransform.localPosition.x);
        float angleDeg = (180 / Mathf.PI) * angleRad + 90;
        connectionPiece.transform.rotation = Quaternion.Euler(0, 0, angleDeg);

        connectionPiece.color = Color.white;
        connections.Add(connectionPiece);
    }

    private void CreateTwoTerrainPieces(GameObject canvas)
    {
        float addedY = canvas.GetComponent<RectTransform>().sizeDelta.y - ((canvas.GetComponent<RectTransform>().sizeDelta.y / 3));
        float currentY = -canvas.GetComponent<RectTransform>().sizeDelta.y / 2 + ((canvas.GetComponent<RectTransform>().sizeDelta.y / 3) / 2);

        for (int i = 0; i < 2; i++)
        {
            Image terrainPiece = Instantiate(image);
            SetUIPosition(terrainPiece.gameObject, canvas, 3, 3, 0, currentY);
            currentY += addedY;
            terrainPiece.color = Color.green;
            terrainPieces.Add(terrainPiece);
        }

        Image connectionPiece = Instantiate(image);
        connectionPiece.transform.SetParent(canvas.transform);
        connectionPiece.rectTransform.localPosition = Vector3.zero;

        Vector2 size = Vector2.zero;
        size.y = terrainPieces[1].rectTransform.localPosition.y - terrainPieces[0].GetComponent<RectTransform>().localPosition.y;
        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / 12;
        connectionPiece.rectTransform.sizeDelta = size;
        connectionPiece.color = Color.white;
        connections.Add(connectionPiece);
    }

    private void ClearMap()
    {
        mapIsActive = false;
        for (int i = 0; i < terrainPieces.Count; i++)
        {
            Destroy(terrainPieces[i].gameObject);
        }
        terrainPieces.Clear();

        for (int i = 0; i < icons.Count; i++)
        {
            Destroy(icons[i].gameObject);
        }
        icons.Clear();

        for (int i = 0; i < information.Count; i++)
        {
            Destroy(information[i].gameObject);
        }
        information.Clear();

        for (int i = 0; i < connections.Count; i++)
        {
            Destroy(connections[i].gameObject);
        }
        connections.Clear();

        Destroy(bg.gameObject);
    }

    private void SetUIPosition(GameObject uiElement, GameObject canvas, float xSize, float ySize, float xPos, float yPos)
    {
        uiElement.transform.SetParent(canvas.transform);

        Vector2 size = Vector2.zero;
        size.y = canvas.GetComponent<RectTransform>().sizeDelta.y / ySize;
        size.x = canvas.GetComponent<RectTransform>().sizeDelta.x / xSize;
        uiElement.GetComponent<RectTransform>().sizeDelta = size;

        Vector3 pos = Vector3.zero;

        pos.y = yPos;

        if (xPos > 0)
        {
            pos.x = xPos + size.x / 2;

        }
        else if (xPos < 0)
        {
            pos.x = xPos - size.x / 2;
        }

        uiElement.GetComponent<RectTransform>().localPosition = pos;
    }

    public void SetTextDescription(Text text, PocketMonsterMoves move)
    {
        PlayerBattle playerBattle = GetComponent<PlayerBattle>();
        Color32 color = Color.black;
        Sprite sprite = null;

        switch (move.moveType)
        {
            case PocketMonsterStats.Typing.Chemical:
                color = new Color32(161, 0, 165, 255);
                sprite = playerBattle.chemicalSprite;

                break;
            case PocketMonsterStats.Typing.Earth:
                color = new Color32(98, 51, 29, 255);
                sprite = playerBattle.earthSprite;
                break;
            case PocketMonsterStats.Typing.Fire:
                color = new Color32(170, 0, 0, 255);
                sprite = playerBattle.fireSprite;
                break;
            case PocketMonsterStats.Typing.Grass:
                color = new Color32(0, 170, 0, 255);
                sprite = playerBattle.grassSprite;
                break;
            case PocketMonsterStats.Typing.Light:
                color = new Color32(184, 155, 0, 255);
                sprite = playerBattle.lightSprite;
                break;
            case PocketMonsterStats.Typing.None:
                break;
            case PocketMonsterStats.Typing.Regular:
                color = new Color32(161, 160, 160, 255);
                sprite = playerBattle.regularSprite;
                break;
            case PocketMonsterStats.Typing.Spooky:
                color = new Color32(106, 0, 166, 255);
                sprite = playerBattle.spookySprite;
                break;
            case PocketMonsterStats.Typing.Static:
                color = new Color32(170, 170, 0, 255);
                sprite = playerBattle.staticSprite;
                break;
            case PocketMonsterStats.Typing.Water:
                color = new Color32(0, 68, 170, 255);
                sprite = playerBattle.waterSprite;
                break;
            case PocketMonsterStats.Typing.Wind:
                color = new Color32(0, 170, 170, 255);
                sprite = playerBattle.windSprite;
                break;
        }

        text.GetComponent<HoverableUiElement>().SetSetInformationWithImages(true);
        text.GetComponent<HoverableUiElement>().SetTypeImageAndAccuracy(sprite, move.accuracy.ToString(), color, move.baseDamage.ToString(),
            move.moveSort);
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void SetTerrainManager(TerrainManager terrainManager)
    {
        this.terrainManager = terrainManager;
    }
}
