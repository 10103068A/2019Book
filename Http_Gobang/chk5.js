function chk5(i, j, k) {
    var n = 0;
    for (m = i - 1; m <= i + 4; m++) {
        n = nn(m, j, k, n);
        if (n == 5) return true;
    }
    n = 0;
    for (m = j - 4; m <= j + 4; m++) {
        n = nn(i, m, k, n);
        if (n == 5) return true;
    }
    n = 0;
    for (m = -4; m < 4; m++) {
        n = nn(i + m, j + m, k, n);
        if (n == 5) return true;
    }
    n = 0;
    for (m = -4; m < 4; m++) {
        n = nn(i - m, j + m, k, n);
        if (n == 5) return true;
    }
    return false;
}
function nn(i, j, k, n) {
    if (i < 0 || i > 18 || j < 0 || j > 18) return n;
    if (A[i][j] == k) {
        return n + 1;
    } else {
        return 0;
    }
}