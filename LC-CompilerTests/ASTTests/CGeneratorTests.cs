using System.Numerics;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using lcc.TypeSystem;
using lcc.Token;
using lcc.SyntaxTree;
using Parserc;

namespace LC_CompilerTests {

    [TestClass]
    public class CGeneratorTests {

        [TestMethod]
        public void LCCCGeneratorTestVoidFunc() {
            Aux("void_func", "void_func_main", "void_func");
        }

        [TestMethod]
        public void LCCCGeneratorTestTypeCast() {
            Aux("type_cast", "type_cast_main", "type_cast");
        }

        [TestMethod]
        public void LCCCGeneratorTestQuickSort() {
            Aux("quick_sort", "quick_sort_main", "quick_sort");
        }

        [TestMethod]
        public void LCCCGeneratorTestHeapSort() {
            Aux("heap_sort", "heap_sort_main", "heap_sort");
        }

        [TestMethod]
        public void LCCCGeneratorTestStackArray() {
            Aux("stack_array", "stack_array_main", "stack_array");
        }

        [TestMethod]
        public void LCCCGeneratorTestQueueArray() {
            Aux("queue_array", "queue_array_main", "queue_array");
        }

        [TestMethod]
        public void LCCCGeneratorTestSinglyLinkedList() {
            Aux("singly_linked_list", "singly_linked_list_main", "singly_linked_list");
        }

        [TestMethod]
        public void LCCCGeneratorTestDoublyLinkedList() {
            Aux("doubly_linked_list", "doubly_linked_list_main", "doubly_linked_list");
        }

        [TestMethod]
        public void LCCCGeneratorTestHomogeneousPool() {
            Aux("homogeneous_pool", "homogeneous_pool_main", "homogeneous_pool");
        }

        [TestMethod]
        public void LCCCGeneratorTestHashTable() {
            Aux("hash_table", "hash_table_main",
                "hash_table",
                "doubly_linked_list"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestBinarySearchTree() {
            Aux("binary_search_tree", "binary_search_tree_main",
                "binary_search_tree",
                "quick_sort"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestRedBlackTree() {
            Aux("rb_tree", "rb_tree_main",
                "rb_tree",
                "quick_sort"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestMatrixChain() {
            Aux("matrix_chain", "matrix_chain_main",
                "matrix_chain"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTRemoveDuplicatesFromSortedArray() {
            Aux("leetcode/remove_duplicates_from_sorted_array", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTRemoveDuplicatesFromSortedArrayII() {
            Aux("leetcode/remove_duplicates_from_sorted_array_2", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTSearchInRotatedSortedArray() {
            Aux("leetcode/search_in_rotated_sorted_array", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTSearchInRotatedSortedArrayII() {
            Aux("leetcode/search_in_rotated_sorted_array_2", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTMedianOfTwoSortedArrays() {
            Aux("leetcode/median_of_two_sorted_arrays", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTLongestConsecutiveSequence() {
            Aux("leetcode/longest_consecutive_sequence", "test",
                "solution",
                "doubly_linked_list",
                "hash_table"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTRemoveElement() {
            Aux("leetcode/remove_element", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTNextPermutation() {
            Aux("leetcode/next_permutation", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTPermutationSequence() {
            Aux("leetcode/permutation_sequence", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTTrappingRainWater() {
            Aux("leetcode/trapping_rain_water", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTRotateImage() {
            Aux("leetcode/rotate_image", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTPlusOne() {
            Aux("leetcode/plus_one", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTCandy() {
            Aux("leetcode/candy", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTSingleNumber() {
            Aux("leetcode/single_number", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTSingleNumberII() {
            Aux("leetcode/single_number_2", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTAddTwoNumbers() {
            Aux("leetcode/add_two_numbers", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTReverseLinkedListII() {
            Aux("leetcode/reverse_linked_list_2", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTRegularExpressionMatching() {
            Aux("leetcode/regular_expression_matching", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTMergeKSortedLists() {
            Aux("leetcode/merge_k_sorted_lists", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTReverseNodesInKGroup() {
            Aux("leetcode/reverse_nodes_in_k_group", "test",
                "solution"
                );
        }

        [TestMethod]
        public void LCCCGeneratorTestLTSubstringWithConcatenationOfAllWords() {
            Aux("leetcode/substring_with_concatenation_of_all_words", "test",
                "solution"
                );
        }

        private void Aux(string name, string test, params string[] srcs) {

            // If the exe exists, delete first.
            Action<string> clear = (fn) => {
                if (File.Exists(fn)) File.Delete(fn);
            };

            var path = "../../ASTTests/code/" + name;
            var main = string.Format("{0}/{1}.c", path, test);
            var main_s = string.Format("{0}/{1}.s", path, test);

            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;

            /// Compile the main.s
            clear(main_s);
            p.StartInfo.FileName = "clang";
            p.StartInfo.Arguments = string.Format("-S {0} -masm=intel -o {1}", main, main_s);
            p.Start();
            p.WaitForExit();

            StringBuilder clang_ss = new StringBuilder();
            StringBuilder lcc_ss = new StringBuilder();

            /// For each src, compile with clang and lcc.
            foreach (var src in srcs) {
                var fn = string.Format("{0}/{1}.c", path, src);
                var lcc_s = string.Format("{0}/{1}_lcc.s", path, src);
                var clang_s = string.Format("{0}/{1}_clang.s", path, src);

                clang_ss.Append(clang_s);
                clang_ss.Append(" ");

                lcc_ss.Append(lcc_s);
                lcc_ss.Append(" ");

                clear(lcc_s);
                clear(clang_s);

                // Compile with lcc.
                File.WriteAllText(lcc_s, Utility.CGen(File.ReadAllText(fn)));

                // Compile with clang.
                p.StartInfo.Arguments = string.Format("-S {0} -masm=intel -o {1}", fn, clang_s);
                p.Start();
                p.WaitForExit();
            }

            var clang_exe = string.Format("{0}/{1}_clang.exe", path, test);
            var lcc_exe = string.Format("{0}/{1}_lcc.exe", path, test);
            clear(clang_exe);
            clear(lcc_exe);

            /// Link them together.
            p.StartInfo.Arguments = string.Format("{0} {1} -o {2}", lcc_ss.ToString(), main_s, lcc_exe);
            p.Start();
            p.WaitForExit();

            p.StartInfo.Arguments = string.Format("{0} {1} -o {2}", clang_ss.ToString(), main_s, clang_exe);
            p.Start();
            p.WaitForExit();

            // Run both program and compare the stdout.
            p.StartInfo.FileName = clang_exe;
            p.StartInfo.Arguments = "";
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            var clang_out = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            p.StartInfo.FileName = lcc_exe;
            p.StartInfo.Arguments = "";
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            var lcc_out = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Assert.AreEqual(clang_out, lcc_out);

            p.Close();
        }

    }
}
