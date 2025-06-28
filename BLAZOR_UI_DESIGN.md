# Blazor UI 설계 문서

## 📋 Blazor 컴포넌트 아키텍처

### 기술 스택
- **Frontend**: Blazor Server
- **UI Framework**: Bootstrap 5
- **Icons**: Bootstrap Icons
- **Authentication**: Blazor Authentication State
- **State Management**: Blazor Services + Dependency Injection

### 컴포넌트 구조
```
Components/
├── Layout/
│   ├── MainLayout.razor          # 메인 레이아웃
│   ├── NavMenu.razor            # 네비게이션 메뉴
│   └── AuthenticationStatus.razor # 인증 상태 표시
├── Pages/
│   ├── Auth/
│   │   ├── Login.razor          # 로그인 페이지
│   │   └── Signup.razor         # 회원가입 페이지
│   ├── Profile.razor            # 프로필 관리
│   ├── Mentors.razor            # 멘토 목록 (멘티만)
│   └── Requests.razor           # 요청 관리 (역할별)
└── Shared/
    ├── LoadingSpinner.razor     # 로딩 스피너
    ├── ErrorAlert.razor         # 오류 알림
    ├── ConfirmDialog.razor      # 확인 대화상자
    └── ImageUpload.razor        # 이미지 업로드
```

---

## 🎨 페이지별 상세 설계

### 1. 로그인 페이지 (`/login`)

#### UI 요구사항 (스펙 준수)
- Email 입력: `id="email"`
- Password 입력: `id="password"`
- 로그인 버튼: `id="login"`

#### 컴포넌트 코드
```razor
@page "/login"
@inject AuthService AuthService
@inject NavigationManager Navigation

<PageTitle>로그인 - 멘토멘티 매칭</PageTitle>

<div class="container-fluid vh-100">
    <div class="row justify-content-center align-items-center h-100">
        <div class="col-md-6 col-lg-4">
            <div class="card shadow-lg">
                <div class="card-header bg-primary text-white text-center">
                    <h4 class="mb-0">
                        <i class="bi bi-person-circle me-2"></i>
                        로그인
                    </h4>
                </div>
                <div class="card-body p-4">
                    <EditForm Model="loginModel" OnValidSubmit="HandleLogin">
                        <DataAnnotationsValidator />
                        
                        <div class="mb-3">
                            <label for="email" class="form-label">이메일</label>
                            <div class="input-group">
                                <span class="input-group-text">
                                    <i class="bi bi-envelope"></i>
                                </span>
                                <InputText id="email" 
                                          class="form-control" 
                                          @bind-Value="loginModel.Email" 
                                          placeholder="이메일을 입력하세요"
                                          disabled="@isLoading" />
                            </div>
                            <ValidationMessage For="() => loginModel.Email" class="text-danger small" />
                        </div>
                        
                        <div class="mb-3">
                            <label for="password" class="form-label">비밀번호</label>
                            <div class="input-group">
                                <span class="input-group-text">
                                    <i class="bi bi-lock"></i>
                                </span>
                                <InputText id="password" 
                                          type="password" 
                                          class="form-control" 
                                          @bind-Value="loginModel.Password" 
                                          placeholder="비밀번호를 입력하세요"
                                          disabled="@isLoading" />
                            </div>
                            <ValidationMessage For="() => loginModel.Password" class="text-danger small" />
                        </div>
                        
                        <button id="login" 
                                type="submit" 
                                class="btn btn-primary w-100 py-2" 
                                disabled="@isLoading">
                            @if (isLoading)
                            {
                                <span class="spinner-border spinner-border-sm me-2"></span>
                                로그인 중...
                            }
                            else
                            {
                                <i class="bi bi-box-arrow-in-right me-2"></i>
                                로그인
                            }
                        </button>
                    </EditForm>
                    
                    @if (!string.IsNullOrEmpty(errorMessage))
                    {
                        <div class="alert alert-danger mt-3 mb-0">
                            <i class="bi bi-exclamation-triangle me-2"></i>
                            @errorMessage
                        </div>
                    }
                </div>
                <div class="card-footer text-center bg-light">
                    <small class="text-muted">
                        계정이 없으신가요? 
                        <a href="/signup" class="text-decoration-none">회원가입</a>
                    </small>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private LoginRequest loginModel = new();
    private bool isLoading = false;
    private string errorMessage = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        // 이미 로그인된 사용자는 프로필 페이지로 리디렉션
        if (await AuthService.IsAuthenticatedAsync())
        {
            Navigation.NavigateTo("/profile");
        }
    }
    
    private async Task HandleLogin()
    {
        try
        {
            isLoading = true;
            errorMessage = string.Empty;
            
            var result = await AuthService.LoginAsync(loginModel);
            if (result.IsSuccess)
            {
                Navigation.NavigateTo("/profile");
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "로그인에 실패했습니다.";
            }
        }
        catch (Exception)
        {
            errorMessage = "로그인 중 오류가 발생했습니다. 다시 시도해주세요.";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}
```

### 2. 회원가입 페이지 (`/signup`)

#### UI 요구사항
- Email 입력: `id="email"`
- Password 입력: `id="password"`
- Role 선택: `id="role"`
- 회원가입 버튼: `id="signup"`

```razor
@page "/signup"
@inject AuthService AuthService
@inject NavigationManager Navigation

<PageTitle>회원가입 - 멘토멘티 매칭</PageTitle>

<div class="container-fluid vh-100">
    <div class="row justify-content-center align-items-center h-100">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow-lg">
                <div class="card-header bg-success text-white text-center">
                    <h4 class="mb-0">
                        <i class="bi bi-person-plus me-2"></i>
                        회원가입
                    </h4>
                </div>
                <div class="card-body p-4">
                    <EditForm Model="signupModel" OnValidSubmit="HandleSignup">
                        <DataAnnotationsValidator />
                        
                        <div class="mb-3">
                            <label for="email" class="form-label">이메일</label>
                            <InputText id="email" 
                                      class="form-control" 
                                      @bind-Value="signupModel.Email" 
                                      placeholder="이메일을 입력하세요" />
                            <ValidationMessage For="() => signupModel.Email" class="text-danger small" />
                        </div>
                        
                        <div class="mb-3">
                            <label for="password" class="form-label">비밀번호</label>
                            <InputText id="password" 
                                      type="password" 
                                      class="form-control" 
                                      @bind-Value="signupModel.Password" 
                                      placeholder="비밀번호를 입력하세요" />
                            <ValidationMessage For="() => signupModel.Password" class="text-danger small" />
                        </div>
                        
                        <div class="mb-3">
                            <label for="name" class="form-label">이름</label>
                            <InputText class="form-control" 
                                      @bind-Value="signupModel.Name" 
                                      placeholder="이름을 입력하세요" />
                            <ValidationMessage For="() => signupModel.Name" class="text-danger small" />
                        </div>
                        
                        <div class="mb-4">
                            <label for="role" class="form-label">역할</label>
                            <InputSelect id="role" 
                                        class="form-select" 
                                        @bind-Value="signupModel.Role">
                                <option value="">역할을 선택하세요</option>
                                <option value="mentor">멘토 (멘티에게 도움을 주는 역할)</option>
                                <option value="mentee">멘티 (멘토에게 도움을 받는 역할)</option>
                            </InputSelect>
                            <ValidationMessage For="() => signupModel.Role" class="text-danger small" />
                        </div>
                        
                        <button id="signup" 
                                type="submit" 
                                class="btn btn-success w-100 py-2" 
                                disabled="@isLoading">
                            @if (isLoading)
                            {
                                <span class="spinner-border spinner-border-sm me-2"></span>
                                가입 중...
                            }
                            else
                            {
                                <i class="bi bi-check-circle me-2"></i>
                                회원가입
                            }
                        </button>
                    </EditForm>
                    
                    @if (!string.IsNullOrEmpty(errorMessage))
                    {
                        <div class="alert alert-danger mt-3 mb-0">@errorMessage</div>
                    }
                </div>
                <div class="card-footer text-center bg-light">
                    <small class="text-muted">
                        이미 계정이 있으신가요? 
                        <a href="/login" class="text-decoration-none">로그인</a>
                    </small>
                </div>
            </div>
        </div>
    </div>
</div>
```

### 3. 프로필 관리 페이지 (`/profile`)

#### UI 요구사항
- Name 입력: `id="name"`
- Bio 입력: `id="bio"`
- Skills 입력: `id="skillsets"` (멘토만)
- 프로필 사진: `id="profile-photo"`
- 프로필 사진 입력: `id="profile"`
- 저장 버튼: `id="save"`

```razor
@page "/profile"
@inject IUserService UserService
@inject IJSRuntime JSRuntime
@attribute [Authorize]

<PageTitle>프로필 관리 - 멘토멘티 매칭</PageTitle>

<div class="container mt-4">
    <div class="row">
        <div class="col-lg-8 mx-auto">
            <div class="card shadow">
                <div class="card-header bg-primary text-white">
                    <h4 class="mb-0">
                        <i class="bi bi-person-gear me-2"></i>
                        프로필 관리
                    </h4>
                </div>
                <div class="card-body p-4">
                    @if (isLoading)
                    {
                        <div class="text-center py-5">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">Loading...</span>
                            </div>
                            <p class="mt-2">프로필을 불러오는 중...</p>
                        </div>
                    }
                    else if (profileModel != null)
                    {
                        <EditForm Model="profileModel" OnValidSubmit="HandleSave">
                            <DataAnnotationsValidator />
                            
                            <div class="row">
                                <!-- 프로필 이미지 -->
                                <div class="col-md-4 text-center mb-4">
                                    <div class="profile-image-container">
                                        <img id="profile-photo" 
                                             src="@GetProfileImageUrl()" 
                                             alt="프로필 사진" 
                                             class="img-fluid rounded-circle shadow"
                                             style="width: 200px; height: 200px; object-fit: cover;" />
                                        <div class="mt-3">
                                            <InputFile id="profile" 
                                                      OnChange="HandleImageChange" 
                                                      accept=".jpg,.jpeg,.png"
                                                      class="form-control"
                                                      style="display: none;" />
                                            <button type="button" 
                                                    class="btn btn-outline-primary btn-sm"
                                                    @onclick="TriggerFileInput">
                                                <i class="bi bi-camera me-1"></i>
                                                사진 변경
                                            </button>
                                        </div>
                                        @if (!string.IsNullOrEmpty(imageError))
                                        {
                                            <div class="text-danger small mt-2">@imageError</div>
                                        }
                                    </div>
                                </div>
                                
                                <!-- 프로필 정보 -->
                                <div class="col-md-8">
                                    <div class="mb-3">
                                        <label for="name" class="form-label">이름</label>
                                        <InputText id="name" 
                                                  class="form-control" 
                                                  @bind-Value="profileModel.Name" 
                                                  placeholder="이름을 입력하세요" />
                                        <ValidationMessage For="() => profileModel.Name" class="text-danger small" />
                                    </div>
                                    
                                    <div class="mb-3">
                                        <label for="bio" class="form-label">자기소개</label>
                                        <InputTextArea id="bio" 
                                                      class="form-control" 
                                                      rows="4"
                                                      @bind-Value="profileModel.Bio" 
                                                      placeholder="자기소개를 입력하세요" />
                                        <ValidationMessage For="() => profileModel.Bio" class="text-danger small" />
                                    </div>
                                    
                                    @if (userRole == "mentor")
                                    {
                                        <div class="mb-3">
                                            <label for="skillsets" class="form-label">기술 스택</label>
                                            <InputText id="skillsets" 
                                                      class="form-control" 
                                                      @bind-Value="skillsInput" 
                                                      placeholder="예: React, Vue, JavaScript (쉼표로 구분)" />
                                            <div class="form-text">쉼표(,)로 구분하여 입력해주세요.</div>
                                            
                                            @if (skills.Any())
                                            {
                                                <div class="mt-2">
                                                    @foreach (var skill in skills)
                                                    {
                                                        <span class="badge bg-primary me-1 mb-1">@skill</span>
                                                    }
                                                </div>
                                            }
                                        </div>
                                    }
                                </div>
                            </div>
                            
                            <hr />
                            
                            <div class="text-center">
                                <button id="save" 
                                        type="submit" 
                                        class="btn btn-primary px-4 py-2" 
                                        disabled="@isSaving">
                                    @if (isSaving)
                                    {
                                        <span class="spinner-border spinner-border-sm me-2"></span>
                                        저장 중...
                                    }
                                    else
                                    {
                                        <i class="bi bi-check-lg me-2"></i>
                                        프로필 저장
                                    }
                                </button>
                            </div>
                        </EditForm>
                        
                        @if (!string.IsNullOrEmpty(successMessage))
                        {
                            <div class="alert alert-success mt-3">@successMessage</div>
                        }
                        
                        @if (!string.IsNullOrEmpty(errorMessage))
                        {
                            <div class="alert alert-danger mt-3">@errorMessage</div>
                        }
                    }
                </div>
            </div>
        </div>
    </div>
</div>
```

### 4. 멘토 목록 페이지 (`/mentors`) - 멘티 전용

#### UI 요구사항
- 개별 멘토 엘리먼트: `class="mentor"`
- 검색 입력: `id="search"`
- 이름 정렬: `id="name"`
- 스킬 정렬: `id="skill"`
- 요청 메시지: `id="message"`, `data-mentor-id="{{mentor-id}}"`, `data-testid="message-{{mentor-id}}"`
- 요청 버튼: `id="request"`

```razor
@page "/mentors"
@inject IMentorService MentorService
@inject IMatchRequestService MatchRequestService
@attribute [Authorize(Roles = "mentee")]

<PageTitle>멘토 찾기 - 멘토멘티 매칭</PageTitle>

<div class="container mt-4">
    <div class="row mb-4">
        <div class="col">
            <h2>
                <i class="bi bi-search me-2"></i>
                멘토 찾기
            </h2>
            <p class="text-muted">당신에게 맞는 멘토를 찾아보세요!</p>
        </div>
    </div>
    
    <!-- 검색 및 필터 -->
    <div class="card mb-4">
        <div class="card-body">
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="search" class="form-label">기술 스택 검색</label>
                    <div class="input-group">
                        <span class="input-group-text">
                            <i class="bi bi-search"></i>
                        </span>
                        <input id="search" 
                               type="text" 
                               class="form-control" 
                               @bind="searchKeyword" 
                               @bind:event="oninput"
                               @onkeypress="HandleSearchKeyPress"
                               placeholder="예: React, Vue, JavaScript" />
                        <button class="btn btn-outline-primary" @onclick="SearchMentors">
                            검색
                        </button>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label">정렬 기준</label>
                    <div class="btn-group w-100" role="group">
                        <input type="radio" class="btn-check" name="sortBy" id="name" @onchange="() => SortMentors('name')" />
                        <label class="btn btn-outline-primary" for="name">이름순</label>
                        
                        <input type="radio" class="btn-check" name="sortBy" id="skill" @onchange="() => SortMentors('skill')" />
                        <label class="btn btn-outline-primary" for="skill">기술순</label>
                    </div>
                </div>
                <div class="col-md-3">
                    <label class="form-label">&nbsp;</label>
                    <button class="btn btn-secondary w-100" @onclick="ResetFilters">
                        <i class="bi bi-arrow-clockwise me-1"></i>
                        초기화
                    </button>
                </div>
            </div>
        </div>
    </div>
    
    <!-- 멘토 목록 -->
    @if (isLoading)
    {
        <div class="text-center py-5">
            <div class="spinner-border text-primary"></div>
            <p class="mt-2">멘토 목록을 불러오는 중...</p>
        </div>
    }
    else if (mentors.Any())
    {
        <div class="row">
            @foreach (var mentor in mentors)
            {
                <div class="col-lg-6 mb-4">
                    <div class="card mentor shadow-sm h-100">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-4 text-center">
                                    <img src="@GetMentorImageUrl(mentor)" 
                                         alt="@mentor.Profile.Name" 
                                         class="img-fluid rounded-circle mb-2"
                                         style="width: 80px; height: 80px; object-fit: cover;" />
                                    <h6 class="text-primary">@mentor.Profile.Name</h6>
                                </div>
                                <div class="col-md-8">
                                    <p class="text-muted mb-2">@mentor.Profile.Bio</p>
                                    <div class="mb-3">
                                        @foreach (var skill in mentor.Profile.Skills ?? new List<string>())
                                        {
                                            <span class="badge bg-light text-dark me-1 mb-1">@skill</span>
                                        }
                                    </div>
                                    
                                    @if (CanSendRequest(mentor.Id))
                                    {
                                        <div class="mb-2">
                                            <textarea id="message" 
                                                     data-mentor-id="@mentor.Id"
                                                     data-testid="message-@mentor.Id"
                                                     class="form-control form-control-sm" 
                                                     rows="2"
                                                     placeholder="멘토링 요청 메시지를 입력하세요"
                                                     @bind="requestMessages[mentor.Id]"></textarea>
                                        </div>
                                        <button id="request" 
                                                class="btn btn-primary btn-sm w-100"
                                                @onclick="() => SendRequest(mentor.Id)">
                                            <i class="bi bi-send me-1"></i>
                                            멘토링 요청
                                        </button>
                                    }
                                    else
                                    {
                                        <div id="request-status" class="alert alert-info mb-0 py-2">
                                            @GetRequestStatus(mentor.Id)
                                        </div>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>
    }
    else
    {
        <div class="text-center py-5">
            <i class="bi bi-search display-1 text-muted"></i>
            <h4 class="text-muted mt-3">검색 결과가 없습니다</h4>
            <p class="text-muted">다른 키워드로 검색해보세요.</p>
        </div>
    }
</div>
```

### 5. 요청 관리 페이지 (`/requests`)

#### 멘토용 UI 요구사항
- 요청 메시지: `class="request-message"`, `mentee="{{mentee-id}}"`
- 수락 버튼: `id="accept"`
- 거절 버튼: `id="reject"`

#### 멘티용 UI는 보낸 요청 목록 표시

```razor
@page "/requests"
@inject IMatchRequestService MatchRequestService
@inject AuthenticationStateProvider AuthStateProvider
@attribute [Authorize]

<PageTitle>요청 관리 - 멘토멘티 매칭</PageTitle>

<div class="container mt-4">
    @if (userRole == "mentor")
    {
        <!-- 멘토: 받은 요청 목록 -->
        <div class="row mb-4">
            <div class="col">
                <h2>
                    <i class="bi bi-inbox me-2"></i>
                    받은 멘토링 요청
                </h2>
                <p class="text-muted">멘티들로부터 받은 멘토링 요청을 관리하세요.</p>
            </div>
        </div>
        
        @if (incomingRequests.Any())
        {
            @foreach (var request in incomingRequests)
            {
                <div class="card mb-3 shadow-sm">
                    <div class="card-body">
                        <div class="row align-items-center">
                            <div class="col-md-2 text-center">
                                <img src="@GetUserImageUrl(request.Mentee)" 
                                     alt="@request.Mentee.Name" 
                                     class="img-fluid rounded-circle"
                                     style="width: 60px; height: 60px; object-fit: cover;" />
                            </div>
                            <div class="col-md-6">
                                <h6 class="mb-1">@request.Mentee.Name</h6>
                                <p class="text-muted small mb-2">@request.Mentee.Bio</p>
                                <div class="request-message" mentee="@request.MenteeId">
                                    <strong>요청 메시지:</strong>
                                    <p class="mb-0">"@request.Message"</p>
                                </div>
                                <small class="text-muted">
                                    요청일: @request.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                                </small>
                            </div>
                            <div class="col-md-2 text-center">
                                <span class="badge bg-@GetStatusColor(request.Status) fs-6">
                                    @GetStatusText(request.Status)
                                </span>
                            </div>
                            <div class="col-md-2 text-end">
                                @if (request.Status == MatchRequestStatus.Pending)
                                {
                                    <div class="btn-group-vertical w-100">
                                        <button id="accept" 
                                                class="btn btn-success btn-sm mb-1"
                                                @onclick="() => AcceptRequest(request.Id)">
                                            <i class="bi bi-check-lg me-1"></i>
                                            수락
                                        </button>
                                        <button id="reject" 
                                                class="btn btn-outline-danger btn-sm"
                                                @onclick="() => RejectRequest(request.Id)">
                                            <i class="bi bi-x-lg me-1"></i>
                                            거절
                                        </button>
                                    </div>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            }
        }
        else
        {
            <div class="text-center py-5">
                <i class="bi bi-inbox display-1 text-muted"></i>
                <h4 class="text-muted mt-3">받은 요청이 없습니다</h4>
                <p class="text-muted">멘티들의 요청을 기다리고 있어요.</p>
            </div>
        }
    }
    else
    {
        <!-- 멘티: 보낸 요청 목록 -->
        <div class="row mb-4">
            <div class="col">
                <h2>
                    <i class="bi bi-send me-2"></i>
                    보낸 멘토링 요청
                </h2>
                <p class="text-muted">내가 보낸 멘토링 요청 상태를 확인하세요.</p>
            </div>
        </div>
        
        @if (outgoingRequests.Any())
        {
            @foreach (var request in outgoingRequests)
            {
                <div class="card mb-3 shadow-sm">
                    <div class="card-body">
                        <div class="row align-items-center">
                            <div class="col-md-2 text-center">
                                <img src="@GetUserImageUrl(request.Mentor)" 
                                     alt="@request.Mentor.Name" 
                                     class="img-fluid rounded-circle"
                                     style="width: 60px; height: 60px; object-fit: cover;" />
                            </div>
                            <div class="col-md-6">
                                <h6 class="mb-1">@request.Mentor.Name</h6>
                                <p class="text-muted small mb-2">@request.Mentor.Bio</p>
                                <div class="mb-2">
                                    @foreach (var skill in request.Mentor.Skills ?? new List<string>())
                                    {
                                        <span class="badge bg-light text-dark me-1">@skill</span>
                                    }
                                </div>
                                <small class="text-muted">
                                    요청일: @request.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                                </small>
                            </div>
                            <div class="col-md-2 text-center">
                                <span class="badge bg-@GetStatusColor(request.Status) fs-6">
                                    @GetStatusText(request.Status)
                                </span>
                            </div>
                            <div class="col-md-2 text-end">
                                @if (request.Status == MatchRequestStatus.Pending)
                                {
                                    <button class="btn btn-outline-danger btn-sm"
                                            @onclick="() => CancelRequest(request.Id)">
                                        <i class="bi bi-x-circle me-1"></i>
                                        취소
                                    </button>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            }
        }
        else
        {
            <div class="text-center py-5">
                <i class="bi bi-send display-1 text-muted"></i>
                <h4 class="text-muted mt-3">보낸 요청이 없습니다</h4>
                <p class="text-muted">
                    <a href="/mentors" class="text-decoration-none">멘토 찾기</a>에서 멘토링을 요청해보세요.
                </p>
            </div>
        }
    }
</div>
```

---

## 🎨 스타일링 및 반응형 디자인

### Bootstrap 5 커스텀 테마
```css
/* wwwroot/css/app.css */

:root {
    --primary-color: #0d6efd;
    --success-color: #198754;
    --warning-color: #ffc107;
    --danger-color: #dc3545;
    --info-color: #0dcaf0;
}

.card {
    border: none;
    box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
    transition: box-shadow 0.15s ease-in-out;
}

.card:hover {
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

.mentor-card {
    transition: transform 0.2s ease-in-out;
}

.mentor-card:hover {
    transform: translateY(-2px);
}

.profile-image-container {
    position: relative;
}

.navbar-brand {
    font-weight: 600;
}

.btn {
    border-radius: 6px;
    font-weight: 500;
}

.form-control:focus {
    border-color: var(--primary-color);
    box-shadow: 0 0 0 0.2rem rgba(13, 110, 253, 0.25);
}

@media (max-width: 768px) {
    .container {
        padding-left: 15px;
        padding-right: 15px;
    }
    
    .card-body {
        padding: 1rem;
    }
}
```

---

## 🔧 상태 관리 및 서비스

### AuthService 
```csharp
public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
    }
    
    public async Task<string> GetUserRoleAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return string.Empty;
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        return jsonToken.Claims.FirstOrDefault(x => x.Type == "role")?.Value ?? string.Empty;
    }
}
```

### 상태 기반 라우팅
```csharp
// Program.cs에서 라우팅 설정
app.MapGet("/", () => Results.Redirect("/login"));
app.MapFallbackToPage("/_Host");
```

이제 모든 설계 문서가 완성되었습니다! 다음 단계로 실제 프로젝트 구조를 생성하시겠습니까?
