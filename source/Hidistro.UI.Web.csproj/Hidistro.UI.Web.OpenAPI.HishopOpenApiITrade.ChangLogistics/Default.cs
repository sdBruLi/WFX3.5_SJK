using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.UI.Web.OpenAPI.Impl;
using Hidistro.Vshop;
using Hishop.Open.Api;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.OpenAPI.HishopOpenApiITrade.ChangLogistics
{
	public class Default : System.Web.UI.Page
	{
		private SiteSettings site = SettingsManager.GetMasterSettings(true);

		private ITrade tradeApi = new TradeApi();

		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			System.Collections.Generic.SortedDictionary<string, string> sortedParams = OpenApiHelper.GetSortedParams(this.Context);
			string empty = string.Empty;
			this.ChangLogistics(sortedParams, ref empty);
			base.Response.ContentType = "text/json";
			Globals.Debuglog(base.Request.Url.ToString() + "||" + Globals.SubStr(empty, 200, "------------结束"), "_DebugERP.txt");
			base.Response.Write(empty);
			base.Response.End();
		}

		public void ChangLogistics(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			if (!this.CheckChangLogisticsParameters(parameters, ref result))
			{
				return;
			}
			if (!OpenApiSign.CheckSign(parameters, this.site.CheckCode, ref result))
			{
				return;
			}
			result = this.tradeApi.ChangLogistics(parameters["tid"], parameters["company_name"], parameters["out_sid"]);
		}

		private bool CheckChangLogisticsParameters(System.Collections.Generic.SortedDictionary<string, string> parameters, ref string result)
		{
			if (!OpenApiHelper.CheckSystemParameters(parameters, this.site.AppKey, out result))
			{
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["tid"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "tid");
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["company_name"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "company_name");
				return false;
			}
			if (!ExpressHelper.IsExitExpress(DataHelper.CleanSearchString(parameters["company_name"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Company_not_Exists, "company_name");
				return false;
			}
			if (string.IsNullOrEmpty(DataHelper.CleanSearchString(parameters["out_sid"])))
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Missing_Required_Arguments, "out_sid");
				return false;
			}
			if (DataHelper.CleanSearchString(parameters["out_sid"]).Length > 20)
			{
				result = OpenApiErrorMessage.ShowErrorMsg(OpenApiErrorCode.Out_Sid_Too_Long, "out_sid");
				return false;
			}
			return true;
		}
	}
}
