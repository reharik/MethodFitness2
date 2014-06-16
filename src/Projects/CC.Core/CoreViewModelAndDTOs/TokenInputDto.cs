using System.Collections.Generic;

namespace CC.Core.CoreViewModelAndDTOs
{
    public class TokenInputDto 
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public interface ITokenInputViewModel { }
    public class TokenInputViewModel : ITokenInputViewModel
    {
        public IEnumerable<TokenInputDto> _availableItems { get; set; }
        public IEnumerable<TokenInputDto> selectedItems { get; set; }
    }

}