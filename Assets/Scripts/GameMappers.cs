using System.Linq;

public static class GameMapper
{
    public static int CharacterToId(Character c) => c.Id;

    public static Character IdToCharacter(int id) =>
        GameController.Instance.AllCharacters.FirstOrDefault(c => c.Id == id);

    public static int ItemToId(Item i) => i.Id;

    public static Item IdToItem(int id) =>
        GameController.Instance.AllItems.FirstOrDefault(i => i.Id == id);
}