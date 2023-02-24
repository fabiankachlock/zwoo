using ZwooGameLogic;
using ZwooGameLogic.Game;

namespace ZwooGamelogicTest;

public class GameManagerTests
{
    // private GameManager gameManager;
    // 
    // [SetUp]
    // public void Setup()
    // {
    // gameManager = new GameManager();
    // }
    // 
    // [Test]
    // public void ShouldInitializeEmpty()
    // {
    // Assert.IsTrue(gameManager.GetAllGames().Count == 0);
    // }
    // 
    // [Test]
    // public void ShouldCreateGame()
    // {
    // Assert.IsTrue(gameManager.GetAllGames().Count == 0);
    // Game game = gameManager.CreateGame("game1", true);
    // Assert.IsTrue(gameManager.GetAllGames().Count == 1);
    // Assert.NotNull(game);
    // Assert.IsTrue(game.Name == "game1");
    // Assert.IsTrue(game.IsPublic);
    // Assert.IsFalse(game.IsRunning);
    // }
    // 
    // [Test]
    // public void ShouldGetGame()
    // {
    // Assert.IsTrue(gameManager.GetAllGames().Count == 0);
    // var g1 = gameManager.CreateGame("game1", true);
    // var g2 = gameManager.CreateGame("game2", true);
    // var g3 = gameManager.CreateGame("game3", true);
    // Assert.IsTrue(gameManager.GetAllGames().Count == 3);
    // Assert.Null(gameManager.GetGame(12376213));
    // Assert.NotNull(gameManager.GetGame(g1.Id));
    // Assert.NotNull(gameManager.GetGame(g2.Id));
    // Assert.NotNull(gameManager.GetGame(g3.Id));
    // Assert.IsTrue(gameManager.GetGame(g1.Id).Name == g1.Name);
    // }
    // 
    // [Test]
    // public void ShouldRemoveGame()
    // {
    // Assert.IsTrue(gameManager.GetAllGames().Count == 0);
    // var g1 = gameManager.CreateGame("game1", true);
    // var g2 = gameManager.CreateGame("game2", true);
    // var g3 = gameManager.CreateGame("game3", true);
    // Assert.IsTrue(gameManager.GetAllGames().Count == 3);
    // Assert.NotNull(gameManager.GetGame(g1.Id));
    // Assert.NotNull(gameManager.GetGame(g2.Id));
    // Assert.NotNull(gameManager.GetGame(g3.Id));
    // Assert.IsTrue(gameManager.RemoveGame(g1.Id));
    // Assert.IsNull(gameManager.GetGame(g1.Id));
    // Assert.IsTrue(gameManager.GetAllGames().Count == 2);
    // Assert.IsTrue(gameManager.RemoveGame(g2.Id));
    // Assert.IsNull(gameManager.GetGame(g2.Id));
    // Assert.IsTrue(gameManager.GetAllGames().Count == 1);
    // Assert.IsTrue(gameManager.RemoveGame(g3.Id));
    // Assert.IsNull(gameManager.GetGame(g3.Id));
    // Assert.IsTrue(gameManager.GetAllGames().Count == 0);
    // }
    // 
    // [Test]
    // public void ShouldFindGame()
    // {
    // Assert.IsTrue(gameManager.GetAllGames().Count == 0);
    // var g1 = gameManager.CreateGame("zwoo", true);
    // var g2 = gameManager.CreateGame("som-sub-zwoo", true);
    // var g3 = gameManager.CreateGame("by-game-id", true);
    // Assert.IsTrue(gameManager.GetAllGames().Count == 3);
    // Assert.IsTrue(gameManager.FindGames("zwoo").Count == 2);
    // Assert.IsTrue(gameManager.FindGames(g3.Id.ToString()).Count == 1);
    // }
}