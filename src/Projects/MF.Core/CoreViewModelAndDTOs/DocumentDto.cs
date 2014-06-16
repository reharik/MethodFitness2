﻿using MF.Core.Domain.Tools.CustomAttributes;

namespace MF.Core.CoreViewModelAndDTOs
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }
        [LinkDisplay]
        public string DocumentName { get; set; }
        public string ViewUrl { get; set; }
        public string DeleteUrl { get; set; }
    } 
}