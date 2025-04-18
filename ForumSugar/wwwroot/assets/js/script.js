// Đăng ký người dùng
document.getElementById('registerForm').addEventListener('submit', function (e) {
    e.preventDefault();
    registerUser(this);
});

async function registerUser(formEl) {
    const name = document.getElementById("regho").value + " " + document.getElementById("reten").value;
    const username = document.getElementById("regUsername").value;
    const email = document.getElementById("regEmail").value;
    const phone = document.getElementById("regPhone").value.trim();
    const dobRaw = document.getElementById("regDob").value;
    const password = document.getElementById("regPassword").value;
    const confirmPassword = document.getElementById("regConfirmPassword").value;
    const messageEl = document.getElementById("message");

    messageEl.innerText = ""; // Xoá thông báo cũ

    // Kiểm tra mật khẩu khớp nhau
    if (password !== confirmPassword) {
        messageEl.innerText = "Mật khẩu không khớp!";
        messageEl.style.color = "red";
        return;
    }

    // Kiểm tra định dạng email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/;
    if (!emailRegex.test(email)) {
        messageEl.innerText = "Email không hợp lệ!";
        messageEl.style.color = "red";
        return;
    }

    // Chuyển đổi ngày sinh về định dạng yyyy-MM-dd
    const dob = new Date(dobRaw).toISOString().split('T')[0];

    // Tạo đối tượng user đúng chuẩn PascalCase (phù hợp ASP.NET)
    const user = {
        Name: name,
        Username: username,
        Email: email,
        PhoneNumber: phone,
        Bornday: dob,
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
        console.log("Phản hồi từ server:", data);

        if (res.ok) {
            messageEl.innerText = "Đăng ký thành công!";
            messageEl.style.color = "green";

            $('#exampleModal').modal('hide');
            formEl.reset();

            // Gửi request login ngay sau khi đăng ký
            const loginRes = await fetch("http://localhost:5093/api/Auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Username: username, Password: password })
            });

            const loginData = await loginRes.json();
            if (loginRes.ok) {
           
                localStorage.setItem("token", loginData.token);

                 //Gọi API /profile để lấy thông tin người dùng
                const profileRes = await fetch("http://localhost:5093/api/Auth/profile", {
                    method: "GET",
                    headers: {
                        "Authorization": `Bearer ${loginData.token}`
                    }
                });

                const profile = await profileRes.json();
                if (profileRes.ok) {
                    loginSuccess(profile); // ⚠️ đảm bảo hàm loginSuccess nhận đúng dữ liệu user
                }

            } else {
                console.log("Login sau đăng ký thất bại:", loginData);
            }
        }
    } catch (error) {
        console.error("Đã có lỗi xảy ra:", error);
        messageEl.innerText = "Đã có lỗi xảy ra.";
        messageEl.style.color = "red";
    }
}

// Đăng nhập người dùng
document.getElementById('LoginForm').addEventListener('submit', function (e) {
    e.preventDefault();
    loginUser(this);
});

async function loginUser() {
    const username = document.getElementById("loginUsername").value;
    const password = document.getElementById("loginPassword").value;
    const messageEl = document.getElementById("message");
    const alertBox = document.getElementById("loginAlert");
    const user = {
        Username: username,
        Password: password,
    };

    try {
        const res = await fetch("http://localhost:5093/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(user)
        });

        const data = await res.json();

        if (res.ok) {
            const token = data.token;
            $('#loginModal').modal('hide'); // Bootstrap 4 cách ẩn modal

            // Hiển thị thông báo thành công
            alertBox.innerText = "Đăng nhập thành công!";
            alertBox.style.backgroundColor = "#4caf50"; // xanh lá
            alertBox.style.display = "block";
            await loginSuccess(token);

            setTimeout(() => {
                alertBox.style.display = "none";
            }, 3000);
        } else {
            // Hiển thị thông báo lỗi từ server
            const errorMessage = data.message || data.error || "Đăng nhập thất bại!";
            messageEl.innerText = errorMessage;
            messageEl.style.color = "red";

            alertBox.innerText = errorMessage;
            alertBox.style.backgroundColor = "#f44336"; // đỏ
            alertBox.style.display = "block";

            setTimeout(() => {
                alertBox.style.display = "none";
            }, 3000);
        }
    } catch (err) {
        messageEl.innerText = "Đã có lỗi xảy ra.";
        messageEl.style.color = "red";

        // Hiển thị lỗi chi tiết từ catch
        alertBox.innerText = err.message || "Đã có lỗi xảy ra.";
        alertBox.style.backgroundColor = "#f44336"; // đỏ
        alertBox.style.display = "block";

        setTimeout(() => {
            alertBox.style.display = "none";
        }, 3000);
    }
}

async function loginSuccess(token) {
    try {
        // Gọi API lấy thông tin user từ token
        const res = await fetch("http://localhost:5093/api/Users/profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!res.ok) {
            throw new Error("Lỗi khi lấy thông tin người dùng!");
        }

        const user = await res.json(); // nhận object user từ server
        console.log("Thông tin người dùng:", user);

        // Ẩn nút login
        document.getElementById('loginBtn').classList.add('d-none');

        // Hiển thị avatar và thông tin
        const userInfo = document.getElementById('user-info');
        const avatar = document.getElementById('user-avatar');

        avatar.src = user.avatarUrl || "/img/default-avatar.png";
        userInfo.classList.remove('d-none');

        // Cập nhật thông tin dropdown
        document.getElementById('popup-avatar').src = user.avatarUrl || "/img/default-avatar.png";
        document.getElementById('popup-username').innerText = user.username;

        // Lưu vào localStorage
        localStorage.setItem("user", JSON.stringify(user));
        localStorage.setItem("token", token);

        // Gán sự kiện Logout
        document.getElementById('logoutBtn').addEventListener('click', () => {
            localStorage.removeItem("user");
            localStorage.removeItem("token");
            location.reload();
        });

    } catch (error) {
        console.error("Lỗi loginSuccess:", error);
        alert("Không thể lấy thông tin người dùng. Vui lòng thử lại!");
    }
}

