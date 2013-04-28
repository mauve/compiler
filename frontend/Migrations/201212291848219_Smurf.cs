namespace frontend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Smurf : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CodeSnippet", "Label", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CodeSnippet", "Label", c => c.String());
        }
    }
}
