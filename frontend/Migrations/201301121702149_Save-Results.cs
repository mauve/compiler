namespace frontend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaveResults : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompilerOutputs", "ExitCode", c => c.Int(nullable: false));
            AddColumn("dbo.CompilerOutputs", "StdOut", c => c.String());
            AddColumn("dbo.CompilerOutputs", "StdErr", c => c.String());
            AddColumn("dbo.CompilerOutputs", "CompileTime", c => c.Time(nullable: false));
            AddColumn("dbo.CompilerOutputs", "MessagesJson", c => c.String());
            DropColumn("dbo.CompilerOutputs", "RawOutput");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompilerOutputs", "RawOutput", c => c.String());
            DropColumn("dbo.CompilerOutputs", "MessagesJson");
            DropColumn("dbo.CompilerOutputs", "CompileTime");
            DropColumn("dbo.CompilerOutputs", "StdErr");
            DropColumn("dbo.CompilerOutputs", "StdOut");
            DropColumn("dbo.CompilerOutputs", "ExitCode");
        }
    }
}
