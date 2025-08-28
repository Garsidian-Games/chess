public interface IStockfishEngine : System.IDisposable {
  void StartSearch(string fen, int depth);
  string ReadLine(); // returns null/empty when no new line
}
