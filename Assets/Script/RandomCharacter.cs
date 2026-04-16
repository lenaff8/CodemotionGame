using UnityEngine;
using UnityEngine;
using System.Collections.Generic;

public class RandomCharacter : MonoBehaviour
{
    [SerializeField] private GameObject normalCardGroup;
    [SerializeField] private GameObject jokerCardGroup;
    [SerializeField] private GameObject gameOverCard;

    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer eyes;
    [SerializeField] private SpriteRenderer hair;
    [SerializeField] private SpriteRenderer head;
    [SerializeField] private SpriteRenderer mouth;
    [SerializeField] private SpriteRenderer suitRank;
    [SerializeField] private SpriteRenderer suitRank2;
    [SerializeField] private SpriteRenderer suitSymbol;
    [SerializeField] private SpriteRenderer suitSymbol2;

    [SerializeField] private SpriteRenderer jokerCharacterRenderer;
    [SerializeField] private SpriteRenderer jokerSuitRenderer;
    [SerializeField] private SpriteRenderer jokerSuitRenderer2;

    [SerializeField] private SpriteRenderer gameOverCardRenderer;
    [SerializeField] private SpriteRenderer gameOverCardSuitRenderer;
    [SerializeField] private SpriteRenderer gameOverCardSuit2Renderer;

    [SerializeField] private List<Sprite> bodySprites = new List<Sprite>();
    [SerializeField] private List<Sprite> eyesSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> hairSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> headSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> mouthSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> rankSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> symbolSprites = new List<Sprite>();

    private int originalBodyOrder;
    private int originalEyesOrder;
    private int originalHairOrder;
    private int originalHeadOrder;
    private int originalMouthOrder;
    private int originalSuitRankOrder;
    private int originalSuitRank2Order;
    private int originalSuitSymbolOrder;
    private int originalSuitSymbol2Order;
    private int originalJokerCharacterOrder;
    private int originalJokerSuitOrder;
    private int originalJokerSuitOrder2;
    private int originalGameOverCardOrder;
    private int originalGameOverCardSuitOrder;
    private int originalGameOverCardSuit2Order; 

    private bool isJokerMode = false;

    private void Start()
    {
        SaveOriginalSortingOrder();
    }

    public void RandomizeCharacter()
    {
        RandomizeCharacter(null, null);
    }

    public void RandomizeCharacter(string role)
    {
        RandomizeCharacter(role, null);
    }

    public void RandomizeCharacter(string role, string type)
    {
        // Cambiar modo si es necesario
        SetCardType(type);

        // Si es joker, no necesita randomizar el personaje
        if (isJokerMode)
            return;

        if (bodySprites.Count > 0)
            body.sprite = bodySprites[Random.Range(0, bodySprites.Count)];

        if (eyesSprites.Count > 0)
            eyes.sprite = eyesSprites[Random.Range(0, eyesSprites.Count)];

        if (hairSprites.Count > 0)
            hair.sprite = hairSprites[Random.Range(0, hairSprites.Count)];

        if (mouthSprites.Count > 0)
            mouth.sprite = mouthSprites[Random.Range(0, mouthSprites.Count)];

        if (headSprites.Count > 0)
            head.sprite = headSprites[Random.Range(0, headSprites.Count)];

        // Asignar palo aleatorio
        if (symbolSprites.Count > 0)
        {
            Sprite randomSymbol = symbolSprites[Random.Range(0, symbolSprites.Count)];
            suitSymbol.sprite = randomSymbol;
            suitSymbol2.sprite = randomSymbol;
        }

        // Asignar n?mero basado en el rol
        SetRankByRole(role);
    }

    private void SetCardType(string type)
    {
        bool shouldBeJoker = !string.IsNullOrEmpty(type) && type.ToLower().Contains("event");
        // Optimización: solo cambiar si el estado es diferente
        if (shouldBeJoker == isJokerMode)
            return;

        isJokerMode = shouldBeJoker;

        if (isJokerMode)
        {
            ShowJokerCardGroup();
        }
        else
        {
            ShowNormalCardGroup();
        }
    }

    private void SetRankByRole(string role)
    {
        int rankIndex = GetRankIndexByRole(role);

        if (rankIndex >= 0 && rankIndex < rankSprites.Count)
        {
            Sprite rankSprite = rankSprites[rankIndex];
            if (suitRank != null)
                suitRank.sprite = rankSprite;
            if (suitRank2 != null)
                suitRank2.sprite = rankSprite;
        }
    }

    private int GetRankIndexByRole(string role)
    {
        if (string.IsNullOrEmpty(role))
            return Random.Range(0, rankSprites.Count);

        // Normalizar el rol a min?sculas para comparaci?n case-insensitive
        role = role.ToLower().Trim();

        if (role.Contains("Company Owner"))
            return 12; // A (?ndice 12 si es 2-10, J, Q, K, A)
        else if (role.Contains("CEO"))
            return 11; // K
        else if (role.Contains("Human Resources"))
            return 10; // Q
        else if (role.Contains("Area Manager"))
            return 9; // J

        return Random.Range(0, 8);
    }

    private void SaveOriginalSortingOrder()
    {
        originalBodyOrder = body.sortingOrder;
        originalEyesOrder = eyes.sortingOrder;
        originalHairOrder = hair.sortingOrder;
        originalHeadOrder = head.sortingOrder;
        originalMouthOrder = mouth.sortingOrder;
        originalSuitRankOrder = suitRank.sortingOrder;
        originalSuitRank2Order = suitRank2.sortingOrder;
        originalSuitSymbolOrder = suitSymbol.sortingOrder;
        originalSuitSymbol2Order = suitSymbol2.sortingOrder;
        originalJokerCharacterOrder = jokerCharacterRenderer.sortingOrder;
        originalJokerSuitOrder = jokerSuitRenderer.sortingOrder;
        originalJokerSuitOrder2 = jokerSuitRenderer2.sortingOrder;
        originalGameOverCardOrder = gameOverCardRenderer.sortingOrder;
        originalGameOverCardSuitOrder = gameOverCardSuitRenderer.sortingOrder;
        originalGameOverCardSuit2Order = gameOverCardSuit2Renderer.sortingOrder;
    }

    public void SetSortingOrderToBack()
    {
        body.sortingOrder = -1;
        eyes.sortingOrder = -1;
        hair.sortingOrder = -1;
        head.sortingOrder = -1;
        mouth.sortingOrder = -1;
        suitRank.sortingOrder = -1;
        suitRank2.sortingOrder = -1;
        suitSymbol.sortingOrder = -1;
        suitSymbol2.sortingOrder = -1;
        jokerCharacterRenderer.sortingOrder = -1;
        jokerSuitRenderer.sortingOrder = -1;
        jokerSuitRenderer2.sortingOrder = -1;
        gameOverCardRenderer.sortingOrder = -1;
        gameOverCardSuitRenderer.sortingOrder = -1;
        gameOverCardSuit2Renderer.sortingOrder = -1;
    }

    public void RestoreSortingOrder()
    {
        body.sortingOrder = originalBodyOrder;
        eyes.sortingOrder = originalEyesOrder;
        hair.sortingOrder = originalHairOrder;
        head.sortingOrder = originalHeadOrder;
        mouth.sortingOrder = originalMouthOrder;
        suitRank.sortingOrder = originalSuitRankOrder;
        suitRank2.sortingOrder = originalSuitRank2Order;
        suitSymbol.sortingOrder = originalSuitSymbolOrder;
        suitSymbol2.sortingOrder = originalSuitSymbol2Order;
        jokerCharacterRenderer.sortingOrder = originalJokerCharacterOrder;
        jokerSuitRenderer.sortingOrder = originalJokerSuitOrder;
        jokerSuitRenderer2.sortingOrder = originalJokerSuitOrder2;
        gameOverCardRenderer.sortingOrder = originalGameOverCardOrder;
        gameOverCardSuitRenderer.sortingOrder = originalGameOverCardSuitOrder;
        gameOverCardSuit2Renderer.sortingOrder = originalGameOverCardSuit2Order;
    }

    public void ShowNormalCardGroup()
    {
        normalCardGroup.SetActive(true);
        jokerCardGroup.SetActive(false);
        gameOverCard.SetActive(false);
        SetSortingOrderToBack();
    }
    public void ShowJokerCardGroup()
    {
        normalCardGroup.SetActive(false);
        jokerCardGroup.SetActive(true);
        gameOverCard.SetActive(false);
        SetSortingOrderToBack();
    }

    public void ShowGameOverCard()
    {
        normalCardGroup.SetActive(false);
        jokerCardGroup.SetActive(false);
        gameOverCard.SetActive(true);
        SetSortingOrderToBack();
    }
}
