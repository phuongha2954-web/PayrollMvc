:root {
    --sbw: 260px;
}

body { background:#f6f7fb; }

.sidebar {
    position:fixed; inset: 0 auto 0 0; width: var(--sbw);
background:#0f172a; color:#cbd5e1; z-index:1030;
    display: flex; flex - direction:column; border - right:1px solid rgba(255,255,255,.08);
}
.brand { padding:18px 20px; display: flex; align - items:center; gap: 10px; color:#fff; text-decoration:none; }
.brand.dot {
width: 10px; height: 10px; border - radius:999px; background:#38bdf8; box-shadow:0 0 12px #38bdf8; }
.menu { padding: 8px }
.menu.nav - link {
    color:#cbd5e1; border-radius:10px; padding:.6rem .8rem; display:flex; align-items:center; gap:.6rem;
}
.menu.nav - link:hover {
    background: rgba(255, 255, 255, .06); color:#fff; }
.menu.nav - link.active {
        background:#1e293b; color:#fff; box-shadow:inset 0 0 0 1px rgba(255,255,255,.08); }
.footer - sb {
                margin - top:auto; padding: 12px 16px; font - size:.9rem; color:#94a3b8; border-top:1px solid rgba(255,255,255,.08); }

.topbar {
                position: sticky; top: 0; z - index:1020; background:#fff; border-bottom:1px solid #e5e7eb; }
.content { margin - left:var(--sbw); padding: 24px; }

                    /* Mobile: ẩn sidebar, thay bằng offcanvas */
                    @media(max - width: 991.98px) {
    .sidebar { display: none; }
    .content { margin - left:0; padding: 16px; }
                    }

/* Extra style cho card & nút */
.card { border - radius:1rem; }
.btn { border - radius:.6rem; }

