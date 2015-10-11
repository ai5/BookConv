//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: book.proto
namespace ShogiLib
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SBook")]
  public partial class SBook : global::ProtoBuf.IExtensible
  {
    public SBook() {}
    
    private string _Author;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Author", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Author
    {
      get { return _Author; }
      set { _Author = value; }
    }
    private string _Description;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Description", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Description
    {
      get { return _Description; }
      set { _Description = value; }
    }
    private readonly global::System.Collections.Generic.List<ShogiLib.SBookState> _BookStates = new global::System.Collections.Generic.List<ShogiLib.SBookState>();
    [global::ProtoBuf.ProtoMember(3, Name=@"BookStates", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ShogiLib.SBookState> BookStates
    {
      get { return _BookStates; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SBookState")]
  public partial class SBookState : global::ProtoBuf.IExtensible
  {
    public SBookState() {}
    
    private int _Id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int Id
    {
      get { return _Id; }
      set { _Id = value; }
    }
    private ulong _BoardKey;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"BoardKey", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ulong BoardKey
    {
      get { return _BoardKey; }
      set { _BoardKey = value; }
    }
    private uint _HandKey;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"HandKey", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint HandKey
    {
      get { return _HandKey; }
      set { _HandKey = value; }
    }
    private int _Games;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"Games", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int Games
    {
      get { return _Games; }
      set { _Games = value; }
    }
    private int _WonBlack;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"WonBlack", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int WonBlack
    {
      get { return _WonBlack; }
      set { _WonBlack = value; }
    }
    private int _WonWhite;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"WonWhite", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int WonWhite
    {
      get { return _WonWhite; }
      set { _WonWhite = value; }
    }
    private string _Position = "";
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"Position", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Position
    {
      get { return _Position; }
      set { _Position = value; }
    }
    private string _Comment = "";
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"Comment", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Comment
    {
      get { return _Comment; }
      set { _Comment = value; }
    }
    private readonly global::System.Collections.Generic.List<ShogiLib.SBookMove> _Moves = new global::System.Collections.Generic.List<ShogiLib.SBookMove>();
    [global::ProtoBuf.ProtoMember(9, Name=@"Moves", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ShogiLib.SBookMove> Moves
    {
      get { return _Moves; }
    }
  
    private readonly global::System.Collections.Generic.List<ShogiLib.SBookEval> _Evals = new global::System.Collections.Generic.List<ShogiLib.SBookEval>();
    [global::ProtoBuf.ProtoMember(10, Name=@"Evals", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ShogiLib.SBookEval> Evals
    {
      get { return _Evals; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SBookMove")]
  public partial class SBookMove : global::ProtoBuf.IExtensible
  {
    public SBookMove() {}
    
    private int _Move;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Move", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int Move
    {
      get { return _Move; }
      set { _Move = value; }
    }
    private ShogiLib.SBookMoveEvalution _Evalution;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Evalution", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ShogiLib.SBookMoveEvalution Evalution
    {
      get { return _Evalution; }
      set { _Evalution = value; }
    }
    private int _Weight;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"Weight", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int Weight
    {
      get { return _Weight; }
      set { _Weight = value; }
    }
    private int _NextStateId;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"NextStateId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int NextStateId
    {
      get { return _NextStateId; }
      set { _NextStateId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SBookEval")]
  public partial class SBookEval : global::ProtoBuf.IExtensible
  {
    public SBookEval() {}
    
    private int _EvalutionValue;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"EvalutionValue", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int EvalutionValue
    {
      get { return _EvalutionValue; }
      set { _EvalutionValue = value; }
    }
    private int _Depth;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"Depth", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int Depth
    {
      get { return _Depth; }
      set { _Depth = value; }
    }
    private int _SelDepth;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"SelDepth", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int SelDepth
    {
      get { return _SelDepth; }
      set { _SelDepth = value; }
    }
    private long _Nodes;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"Nodes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public long Nodes
    {
      get { return _Nodes; }
      set { _Nodes = value; }
    }
    private string _Variation;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"Variation", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string Variation
    {
      get { return _Variation; }
      set { _Variation = value; }
    }
    private string _EngineName = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"EngineName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string EngineName
    {
      get { return _EngineName; }
      set { _EngineName = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"SBookMoveEvalution")]
    public enum SBookMoveEvalution
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"None", Value=0)]
      None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Forced", Value=1)]
      Forced = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Good", Value=2)]
      Good = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Bad", Value=3)]
      Bad = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Blunder", Value=4)]
      Blunder = 4
    }
  
}