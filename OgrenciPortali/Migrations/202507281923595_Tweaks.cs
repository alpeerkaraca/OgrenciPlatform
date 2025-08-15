namespace OgrenciPortali.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tweaks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "DepartmentCode", c => c.String(nullable: false, maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "DepartmentCode");
        }
    }
}
