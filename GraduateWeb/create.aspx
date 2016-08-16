<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" >	
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<meta name="mobile-web-app-capable" content="yes">
   	<title>Create Account</title>
    <!-- css -->
	<link href='<%=Page.ResolveUrl("~/css/base.min.css")%>' rel="stylesheet">
	<link href='<%=Page.ResolveUrl("~/css/project.min.css")%>' rel="stylesheet">
	<link href='<%=Page.ResolveUrl("~/css/site.css")%>' rel="stylesheet">	
</head>
<body class="page-brand">
	<header class="header header-brand ui-header">
		<span class="header-logo">Create Account</span>
	</header>
    <main class="content">
		<div class="container">
			<div class="row">
                <div class="col-lg-4 col-lg-push-4 col-sm-5 col-sm-push-1">
                    <section class="content-inner">
                        <div class="card">
							<div class="card-main">
                                <div id="account-output"></div>
                                <div id="people-output"></div>
                                <div id="social-output"></div>                                
							</div>
						</div>                        
                    </section>    
                </div>
                <div class="col-lg-4 col-lg-push-4 col-sm-5 col-sm-push-1">
                    <section class="content-inner">
                        <div class="card">
							<div class="card-main">
								<div class="card-header">
									<div class="card-inner">
										<h1 class="card-heading">Education</h1>
									</div>
                                    <span class="icon icon-lg" style="padding-right:35px !important;">school</span>
								</div>
                                <div class="card-inner">
                                    <div id="graduation-output"></div>
                                    <div class="clearfix">
                                        <div class="margin-no-top margin-no-bottom pull-right no-margin">          
                                            <button id="btnAddEducation" class="btn btn-flat btn-brand waves-attach waves-effect waves-light">ADD NEW EDUCATION</button>                               
                                        </div>
                                    </div>
                                </div>
                                <div class="card-inner">
                                    <div id="cap-output"></div>
                                </div>
                                <div class="card-inner">
                                    <div id="exchange-output"></div>
                                </div>
                                <div class="card-inner" style="margin-top:96px;">
                                    <div id="workplace-output"></div> 
                                    <div class="clearfix">
                                        <div class="margin-no-top margin-no-bottom pull-right no-margin">                      
                                            <button id="btnAddWorkplace" class="btn btn-flat btn-brand waves-attach waves-effect waves-light">ADD WORKPLACE</button>                               
                                        </div>
                                    </div> 
                                </div>							
                                <div class="clearfix">
                                    <div class="el-loading-done" id="ui_progress">
                                        <div class="el-loading-indicator">
								            <div class="progress progress-position-absolute-top">
                                                <div class="progress-bar-indeterminate"></div>
								            </div>
				                        </div>
                                    </div>  
                                </div>
                            </div>    
						</div>
                        <div class="clearfix">
                            <div class="margin-no-top margin-no-bottom pull-right no-margin">
                                <button id="btnCancelAccount" class="btn btn-flat btn-brand waves-attach waves-effect waves-light">CANCEL</button>
                            </div>
                            <div class="margin-no-top margin-no-bottom pull-right no-margin">
                                <button id="btnSaveAccount" class="btn btn-flat btn-brand waves-attach waves-effect waves-light">SAVE</button>                                                               
                            </div>                           
                        </div>		
                    </section>    
                </div>                
            </div>
        </div>    
    </main>
	<footer class="ui-footer">
		<div class="container">
            <p>MIS Graduation Database</p>
		</div>
	</footer>
    <!-- scripts -->
	<script src='<%=Page.ResolveUrl("~/js/jquery-3.1.0.min.js")%>'></script>
	<script src='<%=Page.ResolveUrl("~/js/ractive.min.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/md5.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/base.min.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/fade.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/create.aspx.js")%>'></script>
</body>
</html>

