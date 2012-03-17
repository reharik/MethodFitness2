﻿using MethodFitness.Core.Domain.Tools.CustomAttributes;

namespace MethodFitness.Core.CoreViewModelAndDTOs
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