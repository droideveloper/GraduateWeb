<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" >	
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<meta name="mobile-web-app-capable" content="yes">
	<!-- title -->
    <title>Reset Password</title>
    <!-- css -->
	<link href='<%=Page.ResolveUrl("~/css/base.min.css")%>' rel="stylesheet">
	<link href='<%=Page.ResolveUrl("~/css/project.min.css")%>' rel="stylesheet">
	<link href='<%=Page.ResolveUrl("~/css/site.css")%>' rel="stylesheet">
</head>
<body class="page-brand">
	<header class="header header-brand ui-header">
		<span class="header-logo">Reset Password</span>
	</header>
	<main class="content">
		<div class="container">
			<div class="row">
				<div class="col-lg-4 col-lg-push-4 col-sm-5 col-sm-push-4">
					<section class="content-inner">                        
						<div class="card">
							<div class="card-main">
								<div class="card-header">
									<div class="card-inner">
										<h1 class="card-heading">Enter New Password</h1>
									</div>
								</div>                                
								<div class="card-inner">								
									<div class="form-group form-group-label">
										<div class="row">
											<div class="col-md-10 col-md-push-1">
												<label class="floating-label" for="ui_password">Password</label>
												<input class="form-control" id="ui_password" type="password">
											</div>
										</div>
									</div>
                                    <div class="form-group form-group-label">
										<div class="row">
											<div class="col-md-10 col-md-push-1">
												<label class="floating-label" for="ui_password_2">Re Password</label>
												<input class="form-control" id="ui_password_2" type="password">
											</div>
										</div>
									</div>
									<div class="form-group">                                        
										<div class="row">
											<div class="col-md-10 col-md-push-1">
												<button id="btnChange" class="btn btn-block btn-brand waves-attach waves-light">Change Password</button>
											</div>                                              
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
	<script src='<%=Page.ResolveUrl("~/js/jquery-3.1.0.min.js")%>'></script>
    <script src='<%=Page.ResolveUrl("~/js/base.min.js")%>'></script>
	<script src='<%=Page.ResolveUrl("~/js/md5.js")%>'></script>	
	<script src='<%=Page.ResolveUrl("~/js/reset.aspx.js")%>'></script>	
</body>
</html>


