
    async function registerUser() {
    const name = document.getElementById("regName").value;
    const username = document.getElementById("regUsername").value;
    const email = document.getElementById("regEmail").value;
    const phone = document.getElementById("regPhone").value;
    const dob = document.getElementById("regDob").value;
    const password = document.getElementById("regPassword").value;
    const confirmPassword = document.getElementById("regConfirmPassword").value;
    const messageEl = document.getElementById("message");

    if (password !== confirmPassword) {
        messageEl.innerText = "Mật khẩu không khớp!";
    messageEl.style.color = "red";
    return;
    }

    const emailRegex = /^[\w.-]+@[a-zA-Z\d.-]+\.[a-zA-Z]{2, 6}$/;
    if (!emailRegex.test(email)) {
        messageEl.innerText = "Email không hợp lệ!";
    messageEl.style.color = "red";
    return;
    }

    const user = {name, username, email, phone, dateOfBirth: dob, password };

    try {
        const res = await fetch("http://localhost:5043/api/auth/register", {
        method: "POST",
    headers: {"Content-Type": "application/json" },
    body: JSON.stringify(user)
        });

    const data = await res.json();
    messageEl.innerText = data.message || "Đăng ký thành công!";
    messageEl.style.color = res.ok ? "green" : "red";
    } catch {
        messageEl.innerText = "Đã có lỗi xảy ra.";
    messageEl.style.color = "red";
    }
}

    async function loginUser() {
    const username = document.getElementById("loginUsername").value;
    const password = document.getElementById("loginPassword").value;
    const messageEl = document.getElementById("message");

    const user = {username, password};

    try {
        const res = await fetch("http://localhost:5043/api/auth/login", {
        method: "POST",
    headers: {"Content-Type": "application/json" },
    body: JSON.stringify(user)
        });

    const data = await res.json();

    if (res.ok) {
        messageEl.innerText = "Đăng nhập thành công!";
    messageEl.style.color = "green";
    console.log("Login success:", data);

    const loginModalEl = document.getElementById('loginModal');
    const modalInstance = bootstrap.Modal.getInstance(loginModalEl);
    modalInstance.hide();
        } else {
        messageEl.innerText = data.message || "Đăng nhập thất bại!";
    messageEl.style.color = "red";
        }
    } catch {
        messageEl.innerText = "Đã có lỗi xảy ra.";
    messageEl.style.color = "red";
    }
}
async function createPost() {
    const title = document.getElementById("postTitle").value;
    const topicInput = document.getElementById("postTopic").value;
    const content = document.getElementById("postContent").value;
    const imgFile = document.getElementById("postImage").files[0]; // Lấy tệp hình ảnh từ input file
    const messageEl = document.getElementById("createMessage");

    // Kiểm tra các trường không được để trống
    if (!title || !content || !topicInput) {
        messageEl.innerText = "Vui lòng nhập tiêu đề, chủ đề và nội dung.";
        messageEl.style.color = "red";
        return;
    }

    // Kiểm tra tệp hình ảnh
    if (imgFile) {
        const validImageTypes = ["image/jpeg", "image/png", "image/gif", "image/bmp"];
        if (!validImageTypes.includes(imgFile.type)) {
            messageEl.innerText = "Vui lòng chọn một tệp hình ảnh hợp lệ (JPEG, PNG, GIF, BMP).";
            messageEl.style.color = "red";
            return;
        }
    }

    // Chuyển các topic từ văn bản thành ID
    const topics = topicInput.split(",").map(topic => topic.trim()); // Tách các chủ đề và loại bỏ khoảng trắng
    const topicIds = await getTopicIds(topics); // Gọi API hoặc tìm kiếm ID của các chủ đề

    try {
        // Tạo FormData để gửi dữ liệu bài viết cùng với hình ảnh
        const formData = new FormData();
        formData.append("title", title);
        formData.append("topicIds", JSON.stringify(topicIds)); // Lưu các topicId dưới dạng chuỗi JSON
        formData.append("content", content);
        if (imgFile) formData.append("imgFile", imgFile); // Thêm hình ảnh vào FormData

        const res = await fetch("http://localhost:5043/api/Blogs", {
            method: "POST",
            body: formData,
        });

        const data = await res.json();
        messageEl.innerText = data.message || "Tạo bài viết thành công!";
        messageEl.style.color = res.ok ? "green" : "red";

        if (res.ok) {
            resetPosts();
        }
    } catch (err) {
        messageEl.innerText = "Đã có lỗi xảy ra.";
        messageEl.style.color = "red";
    }
}

// Hàm hiển thị hình ảnh khi người dùng chọn
document.getElementById("postImage").addEventListener("change", function (event) {
    const file = event.target.files[0];
    const preview = document.getElementById("previewImage");

    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.style.display = "block";
            preview.src = e.target.result; // Hiển thị hình ảnh lên thẻ img
        };
        reader.readAsDataURL(file);
    } else {
        preview.style.display = "none"; // Ẩn hình ảnh nếu không có tệp nào
    }
});

// Hàm giả lập để lấy ID của các topic từ tên
async function getTopicIds(topics) {
    const topicMap = {
        "Công nghệ": 1,
        "Kinh doanh": 2,
        "Giải trí": 3,
        "Thể thao": 4,
        "Giáo dục": 5
    };

    // Tìm ID cho các topic người dùng nhập
    const topicIds = topics.map(topic => topicMap[topic]).filter(id => id !== undefined);

    return topicIds;
}
    let currentPage = 1;
    let isLoading = false;
    let isEnd = false;

    // Function to preview image before uploading
    document.getElementById('postImage').addEventListener('change', function (event) {
            const file = event.target.files[0];
    if (file) {
                const reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById('previewImage').src = e.target.result;
    document.getElementById('previewImage').style.display = 'block';
                };
    reader.readAsDataURL(file);
            }
        });

    // Function to load blog posts
    async function loadPosts() {
            if (isLoading || isEnd) return;
    isLoading = true;
    document.getElementById('loading').style.display = 'block';

    try {
                const res = await fetch(`http://localhost:5093/api/Blogs/scroll?page=${currentPage}&pageSize=10`);
    const data = await res.json();

    if (!data.items || data.items.length === 0) {
        isEnd = true;
    document.getElementById('loading').innerText = "Hết bài viết!";
    return;
                }

                data.items.forEach(post => {
                    const div = document.createElement('div');
    div.classList.add("post-card");
    div.innerHTML = `
    <div style="border: 1px solid #ccc; border-radius: 10px; margin: 10px 0; padding: 10px; box-shadow: 0 2px 6px rgba(0,0,0,0.1)">
        <div style="display: flex; align-items: center; margin-bottom: 10px;">

            <img src="${post.avatarUrl || 'https://khoinguonsangtao.vn/wp-content/uploads/2022/08/hinh-anh-meo-cute-doi-mat-to-tron-den-lay-de-thuong.jpg'}" alt="avatar" style="width: 40px; height: 40px; border-radius: 50%; margin-right: 10px;">
                <div>
                    <strong>${post.authorName || "IT-Zone"}</strong><br />
                    <small>${new Date(post.createdAt).toLocaleDateString()}</small>
                </div>
        </div>
        <div style="font-size: 15px; font-weight: 500; margin-bottom: 5px;">${post.title}</div>
        <div style="margin-bottom: 10px;">${post.content.slice(0, 150)}...</div>
        ${post.imageUrl ? `<img src="${post.imageUrl}" style="max-width: 100%; border-radius: 8px;" />` : ''}
    </div>
                            `;
                    document.getElementById('blog-container').appendChild(div);
                });
                currentPage++;
            } catch (err) {
                console.error(err);
            } finally {
                isLoading = false;
                document.getElementById('loading').style.display = 'none';
            }
        }
        // Event listener for scrolling to load more posts
        window.addEventListener("scroll", () => {
            if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 200 && !isLoading && !isEnd) {
                loadPosts();
            }
        });

        // Initial call to load the first page of posts
        loadPosts();
        // Function to create a new blog post
        async function createPost() {
            const title = document.getElementById('postTitle').value;
            const content = document.getElementById('postContent').value;
            const topic = document.getElementById('postTopic').value;
            const image = document.getElementById('postImage').files[0];

            if (!title || !content) {
                document.getElementById('createMessage').innerText = "Tiêu đề và nội dung không được để trống!";
                return;
            }
            const formData = new FormData();
            formData.append('title', title);
            formData.append('content', content);
            formData.append('topic', topic);
            if (image) formData.append('image', image);

            try {
                const response = await axios.post('http://localhost:5093/api/Blogs', formData, {
                    headers: { 'Content-Type': 'multipart/form-data' }
                });
                document.getElementById('createMessage').innerText = "Bài viết đã được tạo thành công!";
                // Clear form
                document.getElementById('createPostForm').reset();
                document.getElementById('previewImage').style.display = 'none';
            } catch (error) {
                document.getElementById('createMessage').innerText = "Có lỗi xảy ra khi tạo bài viết!";
            }
        }

