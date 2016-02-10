using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MyStoryMaker.Models
{
    public class StoryBlock
    {
        public int id { get; set; }

        [DisplayName("Block Caption")]
        [Required(ErrorMessage = "Enter a caption for the block")]
        public string blockCaption { get; set; }

        [DisplayName("Block Content")]
        [Required(ErrorMessage = "Tell your story")]
        public string blockContent { get; set; }

        [DisplayName("Position")]
        [Required(ErrorMessage = "Give a position to this block")]
        public string bolckOrder { get; set; }

        public string imgPath { get; set; }

    }
}