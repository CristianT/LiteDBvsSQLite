using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common
{
    public class ExampleObject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExampleObjectID  { get; set;}
        public string Name { get; set;}
        public double Value { get; set;}
    }
}
