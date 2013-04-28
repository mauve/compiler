using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace frontend.Models
{
    public class CodeContext : DbContext
    {
        public CodeContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<CodeSnippet> CodeSnippets { get; set; }
        public DbSet<CompilerOutput> CompilerOutputs { get; set; }
    }

    [Table("CodeSnippet")]
    public class CodeSnippet
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Label { get; set; }
        
        [AllowHtml]
        [Required]
        [MaxLength(64000)]
        public string Code { get; set; }
        public CompilerOutput Result { get; set; }
        public virtual bool Finished
        {
            get { return Result != null; }
        }
    }

    public class CompilerOutput
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public bool Successful { get; set; }
        [Required]
        public int ExitCode { get; set; }
        public string StdOut { get; set; }
        public string StdErr { get; set; }
        public TimeSpan CompileTime { get; set; }
        public string MessagesJson { get; set; }
    }
}
