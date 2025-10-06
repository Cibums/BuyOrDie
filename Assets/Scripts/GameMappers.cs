using System.Linq;

public static class GameMapper
{
    public static int CharacterToId(Character c) => GameController.Instance.AllCharacters.ToList().IndexOf(c);

    public static Character IdToCharacter(int id) =>
        GameController.Instance.AllCharacters.ElementAtOrDefault(id);

    public static int ItemToId(Item i) => GameController.Instance.AllItems.ToList().IndexOf(i);

    public static Item IdToItem(int id) =>
        GameController.Instance.AllItems.ElementAtOrDefault(id);
}