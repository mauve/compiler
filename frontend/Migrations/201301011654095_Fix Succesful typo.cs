namespace frontend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSuccesfultypo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompilerOutputs", "Successful", c => c.Boolean(nullable: false));
            DropColumn("dbo.CompilerOutputs", "Succesful");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompilerOutputs", "Succesful", c => c.Boolean(nullable: false));
            DropColumn("dbo.CompilerOutputs", "Successful");
        }
    }
}
