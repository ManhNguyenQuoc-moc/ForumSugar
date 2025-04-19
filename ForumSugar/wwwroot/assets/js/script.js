
document.addEventListener("DOMContentLoaded", () => {
    const token = sessionStorage.getItem('token');
    if (token) loginSuccess(token);
    loadBlogs();
    const blogContainer = document.getElementById("blogContainer");
    blogContainer.addEventListener("scroll", () => {
        if (blogContainer.scrollTop + blogContainer.clientHeight >= blogContainer.scrollHeight - 200) {
            loadBlogs();
        }
    });
    setupAuthForms();
    setupCreatePostModal();
    setupChatBotBtn();
    setupImagePreview();
    setupProfile();
});
function setupProfile() {
    const btnProfile = document.getElementById('btn_profile');
    if (btnProfile) {
        btnProfile.addEventListener('click', function () {
            const token = sessionStorage.getItem('token'); // Lấy token từ sessionStorage
            if (!token) {
                $('#loginPromptModal').modal('show'); // Nếu chưa có token, yêu cầu đăng nhập
                return;
            }
            // Thực hiện gọi API để lấy thông tin người dùng
            fetch('http://localhost:5093/api/Users/profile', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}` // Gửi token trong header Authorization
                }
            })
                .then(res => res.json())
                .then(user => {
                    document.getElementById('rankingchangeprofile').innerHTML = renderUserProfile(user);
                })
                .catch(err => console.error('Error fetching user:', err));
            loadBlogsWithUser();
        });
    }
}

function setupAuthForms() {
    document.getElementById('logoutBtn')?.addEventListener('click', handleLogout);

    document.getElementById('LoginForm')?.addEventListener('submit', (e) => {
        e.preventDefault();
        loginUser();
    });

    document.getElementById('registerForm')?.addEventListener('submit', (e) => {
        e.preventDefault();
        registerUser();
    });
}

function setupCreatePostModal() {
    document.getElementById('createPostBtn')?.addEventListener('click', async () => {
        const token = sessionStorage.getItem('token');
        if (!token) return $('#loginPromptModal').modal('show');

        $('#postModal').modal('show');
        const user = JSON.parse(sessionStorage.getItem("user") || "{}");

        document.getElementById('avatar-user').src = user.avatarUrl || "/img/default-avatar.png";
        document.getElementById('name-user').innerText = user.name || "Người dùng";
        document.getElementById('displayUsername').innerText = "@" + (user.username || "unknown");

        await loadTopicsToSelect();

        document.getElementById("submitPostBtn").onclick = async () => {
            await createPost();
        };
    });
}

function setupChatBotBtn() {
    document.getElementById('chatBotBtn')?.addEventListener('click', () => {
        const token = sessionStorage.getItem('token');
        token ? $('#chatModal').modal('show') : $('#loginPromptModal').modal('show');
    });
}

function setupImagePreview() {
    document.getElementById("postImage")?.addEventListener("change", (event) => {
        const file = event.target.files[0];
        const preview = document.getElementById("previewImage");

        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                preview.style.display = "block";
                preview.src = e.target.result;
            };
            reader.readAsDataURL(file);
        } else {
            preview.style.display = "none";
        }
    });
}

// ========== AUTH ==========
async function registerUser() {
    const name = document.getElementById("regho").value + " " + document.getElementById("reten").value;
    const username = document.getElementById("regUsername").value;
    const email = document.getElementById("regEmail").value;
    const phone = document.getElementById("regPhone").value.trim();
    const dobRaw = document.getElementById("regDob").value;
    const password = document.getElementById("regPassword").value;
    const confirmPassword = document.getElementById("regConfirmPassword").value;
    const messageEl = document.getElementById("message");
    messageEl.innerText = "";

    if (password !== confirmPassword) return showMessage(messageEl, "Mật khẩu không khớp!", "red");
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/;
    if (!emailRegex.test(email)) return showMessage(messageEl, "Email không hợp lệ!", "red");

    const user = {
        Name: name,
        Username: username,
        Email: email,
        PhoneNumber: phone,
        Bornday: new Date(dobRaw).toISOString().split('T')[0],
        Password: password,
        ConfirmPassword: confirmPassword
    };

    try {
        const res = await fetch("http://localhost:5093/api/Auth/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(user)
        });
        const data = await res.json();

        if (res.ok) {
            showMessage(messageEl, "Đăng ký thành công!", "green");
            $('#exampleModal').modal('hide');
            document.getElementById("registerForm").reset();
            await autoLoginAfterRegister(username, password);
        } else {
            showMessage(messageEl, data.message || "Đăng ký thất bại!", "red");
        }
    } catch (error) {
        console.error(error);
        showMessage(messageEl, "Đã có lỗi xảy ra.", "red");
    }
}

async function autoLoginAfterRegister(username, password) {
    try {
        const res = await fetch("http://localhost:5093/api/Auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Username: username, Password: password })
        });
        const data = await res.json();
        if (res.ok) {
            sessionStorage.setItem("token", data.token);
            await loginSuccess(data.token);
        }
    } catch (err) {
        console.error("Lỗi tự đăng nhập:", err);
    }
}
async function loginUser() {
    const username = document.getElementById("loginUsername").value;
    const password = document.getElementById("loginPassword").value;
    const alertBox = document.getElementById("loginAlert");
    const messageEl = document.getElementById("message");

    try {
        const res = await fetch("http://localhost:5093/api/Auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Username: username, Password: password })
        });

        const data = await res.json();
        if (res.ok) {
            sessionStorage.setItem("token", data.token);
            $('#loginModal').modal('hide');
            await loginSuccess(data.token);
            showAlert(alertBox, "Đăng nhập thành công!", "#4caf50");
        } else {
            showMessage(messageEl, data.message || "Đăng nhập thất bại!", "red");
            showAlert(alertBox, data.message, "#f44336");
        }
    } catch (err) {
        showMessage(messageEl, "Đã có lỗi xảy ra.", "red");
        showAlert(alertBox, err.message || "Lỗi khi đăng nhập.", "#f44336");
    }
}

async function loginSuccess(token) {
    try {
        const res = await fetch("http://localhost:5093/api/Users/profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!res.ok) throw new Error("Không thể lấy thông tin người dùng!");
        const user = await res.json();
        sessionStorage.setItem("user", JSON.stringify(user));
        document.getElementById('loginBtn').classList.add('d-none');
        const avatarUrl = user.avatarUrl || "/img/default-avatar.png";
        document.getElementById('user-avatar').src = avatarUrl;
        document.getElementById('popup-avatar').src = avatarUrl;
        document.getElementById('popup-username').innerText = "@" + user.username;
        document.getElementById("popup-name").innerText = user.name;
        document.getElementById('user-info').classList.remove('d-none');
        resetPosts();
    } catch (err) {
        console.error("Lỗi loginSuccess:", err);
        alert("Không thể lấy thông tin người dùng. Vui lòng thử lại!");
    }
}

function handleLogout() {
    sessionStorage.removeItem("token");
    sessionStorage.removeItem("user");
    location.reload();
}

// ========== POST ==========
async function loadTopicsToSelect() {
    try {
        const res = await fetch("http://localhost:5093/api/Topic");
        const data = await res.json();

        if (data.success && Array.isArray(data.data)) {
            const select = document.getElementById("topicSelect");
            select.innerHTML = "";
            data.data.forEach(topic => {
                const opt = document.createElement("option");
                opt.value = topic.id;
                opt.textContent = topic.name;
                select.appendChild(opt);
            });
        }
    } catch (err) {
        console.error("Lỗi load topic:", err);
    }
}

async function createPost() {
    const title = document.getElementById("postTitle").value;
    const topicId = document.getElementById("topicSelect").value;
    const content = document.getElementById("postContent").value;
    const imgFile = document.getElementById("postImage").files[0];
    const alertBox = document.getElementById("alertBox");
    const token = sessionStorage.getItem('token');
    const user = JSON.parse(sessionStorage.getItem("user") || "{}");

    if (!title || !content || !topicId || !user.id) {
        return showAlert(alertBox, "Vui lòng nhập đủ thông tin và đăng nhập.", "red");
    }

    if (imgFile && !["image/jpeg", "image/png", "image/gif", "image/bmp"].includes(imgFile.type)) {
        return showAlert(alertBox, "Tệp hình ảnh không hợp lệ.", "red");
    }

    try {
        const formData = new FormData();
        formData.append("Title", title);
        formData.append("Content", content);
        formData.append("TopicId", topicId);
        formData.append("UserId", user.id);
        if (imgFile) formData.append("Image", imgFile);

        const res = await fetch("http://localhost:5093/api/Blogs", {
            method: "POST",
            headers: { "Authorization": `Bearer ${token}` },
            body: formData
        });

        if (res.ok) {
            showAlert(alertBox, "Tạo bài viết thành công!", "green");
            resetFormCreatePosts();
            $('#postModal').modal('hide');
            resetPosts();
        }
    } catch (err) {
        console.error(err);
        showAlert(alertBox, "Đã có lỗi xảy ra.", "red");
    }
}

// ========== BLOG SCROLL ==========
let page = 1;
let isLoading = false;
let reachedEnd = false;
async function loadBlogs() {
    if (isLoading || reachedEnd) return;
    isLoading = true;
    const user = JSON.parse(sessionStorage.getItem("user") || null);
    try {
        let res;
        if (!user || !user.id) {
            res = await fetch(`http://localhost:5093/api/Blogs/scroll?page=${page}&pageSize=10`);
        } else {
            res = await fetch(`http://localhost:5093/api/Blogs/scroll?page=${page}&pageSize=10&currentUserId=${user.id}`);
        }

        if (!res.ok) throw new Error('Lỗi khi tải dữ liệu');

        const data = await res.json();

        if (data.items.length === 0 && page === 1) {
            document.getElementById("blogContainer").innerHTML = `<p class="text-center mt-3 text-muted">Không có bài viết nào</p>`;
            reachedEnd = true;
            return;
        }
        const container = document.getElementById('blogContainer');
        data.items.forEach(post => {
            container.insertAdjacentHTML('beforeend', renderPostHtml(post));
            document.getElementById(`likeCount_${post.id}`).addEventListener('click', () => {
                const user = JSON.parse(sessionStorage.getItem("user") || null);
                if (user?.id) {
                    LikeandUnlike(user.id, post.id);
                } else {
                    if (!sessionStorage.getItem('token')) {
                        $('#loginPromptModal').modal('show');
                    }
                }
            });
        });

     const pageSize = 10;
    if ((page * pageSize) >= data.totalRecords) {
        reachedEnd = true;
        container.insertAdjacentHTML("beforeend", `<p class="text-center mt-3 text-muted">Đã hết bài viết</p>`);
    }
    page++;
    } catch (err) {
        console.error("Lỗi khi tải blog:", err);
    } finally {
        isLoading = false;
    }
}
async function loadBlogsWithUser() {
    if (isLoading || reachedEnd) return;
    isLoading = true;
    const user = JSON.parse(sessionStorage.getItem("user") || null);
    try {
        let res;
        res = await fetch(`http://localhost:5093/api/Blogs/profile/blogs/?page=${page}&pageSize=10&currentUserId=${user.id}`);
        if (!res.ok) throw new Error('Lỗi khi tải dữ liệu');

        const data = await res.json();

        if (data.items.length === 0 && page === 1) {
            document.getElementById("blogContainer").innerHTML = `<p class="text-center mt-3 text-muted">Không có bài viết nào</p>`;
            reachedEnd = true;
            return;
        }
        const container = document.getElementById('blogContainer');
        data.items.forEach(post => {
            container.insertAdjacentHTML('beforeend', renderPostHtml(post));
            document.getElementById(`likeCount_${post.id}`).addEventListener('click', () => {
                const user = JSON.parse(sessionStorage.getItem("user") || null);
                if (user?.id) {
                    LikeandUnlike(user.id, post.id);
                } else {
                    if (!sessionStorage.getItem('token')) {
                        $('#loginPromptModal').modal('show');
                    }
                }
            });
        });

        const pageSize = 10;
        if ((page * pageSize) >= data.totalRecords) {
            reachedEnd = true;
            container.insertAdjacentHTML("beforeend", `<p class="text-center mt-3 text-muted">Đã hết bài viết</p>`);
        }
        page++;
    } catch (err) {
        console.error("Lỗi khi tải blog:", err);
    } finally {
        isLoading = false;
    }
}
function resetFormCreatePosts() {
    document.getElementById("postTitle").value = "";
    document.getElementById("topicSelect").selectedIndex = 0;
    document.getElementById("postContent").value = "";
    document.getElementById("postImage").value = "";
    document.getElementById("previewImage").src = "/img/upload-placeholder.png";
    document.getElementById("previewImage").style.display = "none";
}
function resetPosts() {
    document.getElementById("blogContainer").innerHTML = "";
    page = 1;
    reachedEnd = false;
    loadBlogs();
}

function renderPostHtml(post) {
    return `
    <div class="card mb-3">
        <div class="card-body">
            <div class="d-flex align-items-center mb-2">
                <img src="${post.avatarUrl}" alt="Avatar" width="40" height="40" class="rounded-circle me-2" />
                <div>
                    <strong>${post.authorName}</strong><br />
                    <small class="text-muted">${new Date(post.createdAt).toLocaleString()}</small>
                </div>
            </div>
            <h5>${post.title}</h5>
            <p>${post.content}</p>
            ${post.imageUrl ? `<img src="${post.imageUrl}" class="img-fluid rounded" alt="Post Image" />` : ""}
            <div class="mt-2">
                <div class="d-flex justify-content-between align-items-center mt-2">
                    <div>
                        <button id="likeCount_${post.id}" style="color: black; font-size: 15px; font-family: Quicksand; font-weight: 600;">
                            <i class="${post.isLikedByCurrentUser ? 'fa-solid' : 'fa-regular'} fa-heart" style="color: ${post.isLikedByCurrentUser ? '#ff0000' : ''};"></i> ${post.likeCount}
                        </button>
                        <button id="commentCount_${post.id}" style="color: black; font-size: 15px; font-family: Quicksand; font-weight: 600; line-height: 22px; word-wrap: break-word;">
                            <i class="fa-regular fa-comment"></i> ${post.commentCount}
                        </button>
                        <button id="savebtn_${post.id}" style="color: black; font-size: 15px; font-family: Quicksand; font-weight: 600; line-height: 22px; word-wrap: break-word;">
                            <i class="fa-regular fa-bookmark"></i> Lưu
                        </button>
                    </div>
                    <button id="reportBtn_${post.id}" class="btn btn-sm btn-outline-danger" style="border: none; color: black; font-size: 15px; font-family: Quicksand; font-weight: 600; line-height: 22px; word-wrap: break-word;">
                        <i class="fa-solid fa-triangle-exclamation"></i> Báo cáo
                    </button>
                </div>
            </div>
        </div>
   </div>
  `; 
}
async function LikeandUnlike(userId, postId) {
    // Kiểm tra nếu userId và postId hợp lệ
    if (!userId || !postId) {
        console.error("User ID hoặc Post ID không hợp lệ");
        return;
    }
    try {
        // Gọi API để thực hiện hành động like/unlike
        const res = await fetch(`http://localhost:5093/api/Blogs/like/${postId}?userId=${userId}`, {
            method: 'POST', // Gọi với phương thức POST để thực hiện like/unlike
            headers: {
                'Content-Type': 'application/json'
            }
        });

        // Kiểm tra phản hồi từ server
        if (!res.ok) {
            throw new Error('Lỗi khi thực hiện like/unlike');
        }

        const data = await res.json();

        // Kiểm tra kết quả trả về từ API, ví dụ: trạng thái like mới hoặc thông báo thành công
        if (data.success) {
            console.log(`Like/Unlike thành công cho bài viết ${postId}`);

            // Cập nhật giao diện
            updateLikeUI(postId, data.data.isLiked, data.data.likeCount);
        } else {
            console.log('Có lỗi khi thực hiện hành động');
        }
    } catch (error) {
        console.error('Lỗi khi gọi API:', error);
    }
}

// Hàm cập nhật giao diện (có thể thay đổi tùy vào yêu cầu)
function updateLikeUI(postId, isLiked, likeCount) {
    const heartIcon = document.getElementById(`likeCount_${postId}`);
    if (heartIcon) {
        // Thay đổi icon tim và màu sắc
        heartIcon.innerHTML = `
            <i class="${isLiked ? 'fa-solid' : 'fa-regular'} fa-heart" style="color: ${isLiked ? '#ff0000' : ''};"></i> ${likeCount}
        `;
    }
}
//Profile
function renderUserProfile(user) {
    return `
    <div class="position" style="top: 20px;">
        <div class="card mx-auto shadow-sm border border-info" style="max-width: 400px; border-radius: 20px;">
            <div class="card-body text-center">
                <img src="${user.avatarUrl}" class="rounded-circle mb-3" width="100" height="100" style="object-fit: cover;">
                <h5 class="card-title mb-1 font-weight-bold">${user.fullName}</h5>
                <p class="text-muted mb-1">@${user.username}</p>
                <p class="mb-2">Email: ${user.email}</p>
                <div class="d-flex justify-content-center text-muted small mb-3">
                    <span class="mx-2">${user.postsCount || 0} posts</span>
                    <span class="mx-2">${user.followersCount || 0} followers</span>
                    <span class="mx-2">${user.followingCount || 0} following</span>
                </div>
                <div class="text-left mb-3">
                    <h6 class="font-weight-semibold mb-1">Intro</h6>
                    <div class="bg-light p-2 rounded border">${user.bio || 'Không có mô tả.'}</div>
                </div>
                <div class="text-left">
                    <h6 class="font-weight-semibold mb-1">Thành tựu</h6>
                    <div class="d-flex flex-wrap">
                        ${Array.isArray(user.achievements)
            ? user.achievements.map(a => `
                                    <span class="badge badge-pill badge-light border border-secondary text-dark px-3 py-2 m-1">
                                        ${a}
                                    </span>
                                `).join('')
            : '<span class="text-muted">Không có thành tựu nào</span>'
        }
                    </div>
                </div>
            </div>
        </div>
    </div>
    `;
}


//document.getElementById('blogContainer').innerHTML = renderUserProfile(user);
// ========== UTILS ==========
function showMessage(el, message, color) {
    el.innerText = message;
    el.style.color = color;
}

function showAlert(el, message, bgColor) {
    el.innerText = message;
    el.style.backgroundColor = bgColor;
    el.style.display = "block";
    setTimeout(() => { el.style.display = "none"; }, 3000);
}
