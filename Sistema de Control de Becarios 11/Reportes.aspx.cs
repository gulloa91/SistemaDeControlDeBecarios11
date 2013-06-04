using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reportes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MultiViewReportes.ActiveViewIndex = 0;
    }

    // Click del Menu
    protected void MenuListaReportes_MenuItemClick(object sender, MenuEventArgs e)
    {
        switch (e.Item.Text)
        {
            case "":
                {

                } break;
        }
    }
}