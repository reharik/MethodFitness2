namespace CC.Security.Interfaces
{
	/// <summary>
	/// Mark an entity with an id
	/// </summary>
	public interface IIDentifiable
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
        int EntityId { get; set; }
	}
}