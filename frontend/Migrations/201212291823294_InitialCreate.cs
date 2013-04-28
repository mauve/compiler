namespace frontend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CodeSnippet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                        Code = c.String(nullable: false),
                        Result_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompilerOutputs", t => t.Result_Id)
                .Index(t => t.Result_Id);
            
            CreateTable(
                "dbo.CompilerOutputs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Succesful = c.Boolean(nullable: false),
                        RawOutput = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CodeSnippet", new[] { "Result_Id" });
            DropForeignKey("dbo.CodeSnippet", "Result_Id", "dbo.CompilerOutputs");
            DropTable("dbo.CompilerOutputs");
            DropTable("dbo.CodeSnippet");
        }
    }
}
