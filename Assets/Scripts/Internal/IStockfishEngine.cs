public interface IStockfishEngine : System.IDisposable {
  bool HasBestMove { get; }
  string BestMove { get; }
  void StartSearch(string fen, int depth);
  void Update();
}
