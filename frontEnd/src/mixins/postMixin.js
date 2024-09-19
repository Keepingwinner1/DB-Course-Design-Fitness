import axios from 'axios';

export const postMixin = {
    methods: {

        getCommentCount(postID) {
            const token = this.$store.state.token;
            return axios.get('http://localhost:8080/api/Comment/GetCommentByPostID', {
                params: { postID: postID },
                headers: {
                    Authorization: `Bearer ${token}`
                }
            })
                .then(response => {
                    const comments = response.data;
                    const countComments = (comments) => {
                        return comments.reduce((acc, comment) => {
                            return acc + 1 + countComments(comment.replies);
                        }, 0);
                    };
                    return countComments(comments);
                })
                .catch(error => {
                    console.error('获取评论时发生错误:', error);
                    return 0;
                });
        },

        viewPost(postID) {
            this.$router.push({ name: 'PostDetail', params: { postID: postID } });
        }
    }
};
