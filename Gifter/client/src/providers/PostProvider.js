import React, { useState } from "react";

export const PostContext = React.createContext();

export const PostProvider = (props) => {
    const [posts, setPosts] = useState([]);
    const [postsWithComments, setPostsWithComments] = useState([]);

    const getAllPosts = () => {
        return fetch("/api/post")
            .then((res) => res.json())
            .then(setPosts);
    };

    const getPostsWithComments = () => {
        return fetch("/api/post/getwithcomments")
            .then((res) => res.json())
            .then(setPostsWithComments);
    };

    const addPost = (post) => {
        return fetch("/api/post", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(post),
        });
    };

    const getPost = (id) => {
        return fetch(`/api/post/${id}`).then((res) => res.json());
    };

    return (
        <PostContext.Provider value={{ posts, postsWithComments, getAllPosts, addPost, getPost, getPostsWithComments }}>
            {props.children}
        </PostContext.Provider>
    );
};