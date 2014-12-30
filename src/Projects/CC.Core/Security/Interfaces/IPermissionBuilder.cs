using CC.Core.Security.Model;

namespace CC.Core.Security.Interfaces
{
	/// <summary>
	/// Save the created permission
	/// </summary>
	public interface IPermissionBuilder
	{
       /// <summary>
       /// 
       /// </summary>
       /// <param name="description">the description for this permission</param>
        /// <returns>IPermissionBuilder</returns>
	    IPermissionBuilder Description(string description);
		/// <summary>
		/// Save the created permission
		/// </summary>
		Permission Save();
        
        /// <summary>
		/// Builds a permission without saving
		/// </summary>
		Permission Build();
	}
}