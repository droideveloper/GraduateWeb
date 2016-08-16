<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" >	
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<meta name="mobile-web-app-capable" content="yes">
   	<title>Search Graduates</title>
    <!-- css -->
	<link href='<%=Page.ResolveUrl("~/css/base.min.css")%>' rel="stylesheet">
	<link href='<%=Page.ResolveUrl("~/css/project.min.css")%>' rel="stylesheet">
    <link href='<%=Page.ResolveUrl("~/css/site.css")%>' rel="stylesheet">
</head>
<body class="page-brand">
    <header class="header header-brand header-standard ui-header">
        <span class="header-logo">Search Graduates</span>
        <div class="col-md-1 pull-right margin-top-xs">
            <a class="btn btn-flat waves-attach waves-effect waves-light pull-right text-center" data-toggle="modal" href="#ui_workplace_dialog">
                <span class="icon icon-lg">work</span>
            </a>  
        </div>
    </header>
    <main class="content ui-picker-page">
        <div class="ui-picker-wrap">
            <div class="ui-picker-main">            
                <div class="ui-picker-inner">
                    <div class="clearfix">
                        <div class="el-loading-done" id="ui_progress">
                            <div class="el-loading-indicator">
                                <div class="progress progress-position-absolute-top">
                                    <div class="progress-bar-indeterminate"></div>
                                </div>
                            </div>
                        </div>  
                    </div>
                    <div class="form-group form-group-label">                        
                        <div class="row">
                            <div class="col-md-7 col-md-push-1">               
                                <label class="floating-label" for="uiSearch"><span class="icon icon-lg">search</span>&nbsp;&nbsp;Name, Surname, Graduate Year...</label>
                                <input class="form-control" id="uiSearch" type="text">
                            </div>
                            <div class="col-md-3 col-md-push-1">
                                <button class="btn btn-flat btn-brand waves-attach waves-effect waves-light" id="btnSearch">SEARCH</button>                                               
                                <span class="margin-left-xs margin-right-xs text-center">OR</span>                         
                                <a class="btn btn-flat btn-brand waves-attach waves-effect waves-light" data-toggle="modal" href="#ui_filter_dialog">FILTER</a>        
                            </div>
                            <div class="col-md-2 col-md-push-1 pull-right">
                                <button class="btn btn-flat btn-brand waves-attach waves-effect waves-light" id="btnClear">CLEAR</button> 
                            </div>
                        </div>                        
                    </div>
                    <div class="tile-wrap margin-bottom-no margin-top-no ui-picker-lib" id="output"></div>
                </div>
            </div>
            <div class="ui-picker-divider"></div>
            <div class="ui-picker-info ui-picker-info-null">
                <div class="ui-picker-inner">
                    <div class="container">
                        <div class="media">
                            <div class="media-object pull-right">
                                <a class="btn btn-flat margin-bottom-xs margin-top-xs waves-attach ui-picker-info-close"><span class="icon">close</span></a>
                            </div>
                            <div class="media-inner">
                                <div class="h5 margin-bottom-sm margin-top-sm ui-picker-info-title-wrap"></div>                               
                            </div>
                        </div>                        
                    </div>
                    <div class="col-xx-12 margin-bottom-no margin-top-no hr margin-left-no"></div>
                    <div class="container padding-left-no padding-right-no">
                        <div class="ui-picker-info-desc-wrap"></div>
                    </div>
                </div>
            </div>
        </div> 
        <div id="filter-output"></div>
        <div id="workplace-output"></div>       
    </main>
    <footer class="ui-footer">
        <div class="container">
           <p>MIS Graduation Database</p>
        </div>
	</footer>   
    <script src='<%=Page.ResolveUrl("~/js/jquery-3.1.0.min.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/jquery-ui.min.js")%>'></script>
	<script src='<%=Page.ResolveUrl("~/js/ractive.min.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/base.min.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/fade.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/query.aspx.js")%>'></script>
</body>    

