﻿using System.Collections.Generic;

namespace MethodFitness.Core.CoreViewModelAndDTOs
{
    public class TokenInputDto
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class TokenInputViewModel
    {
        public IEnumerable<TokenInputDto> _availableItems { get; set; }
        public IEnumerable<TokenInputDto> selectedItems { get; set; }
    }
}